using Blazored.LocalStorage;
using Blazored.SessionStorage;
using FSAuth.Core;
using FSCommon;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;

namespace CM.Shared {

    /// <summary>
    /// Top-right account widget: who is signed in, the theme picker, and Logout.
    ///
    /// A layout-level widget has no parent page to hand it AppState, and it renders before the
    /// page has finished loading its own. So it attaches by itself to the same tab-scoped
    /// instance (AppStateManager returns the one the page uses) and shows a skeleton until a user
    /// is attached. It never redirects - an unauthenticated visit is the page's call to make.
    /// </summary>
    public class LoggedInUser_Base : ComponentBase, IDisposable {

        #region Injected Services
        [Inject] protected IJSRuntime JS { get; set; } = default!;
        [Inject] protected ISessionStorageService _sessionStorage { get; set; } = default!;
        [Inject] protected ILocalStorageService _localStorage { get; set; } = default!;
        [Inject] protected IAppStateManager _appStateManager { get; set; } = default!;
        [Inject] protected NavigationManager _nav { get; set; } = default!;
        [Inject] protected FSModalService _fsModalService { get; set; } = default!;
        [Inject] protected ISnackbar _toasterService { get; set; } = default!;
        [Inject] protected IFSAuthenticationService _authService { get; set; } = default!;
        #endregion (Injected Services)

        #region Fields
        protected AppState_CM? _appState;
        protected bool _isHydrated;
        protected bool _isOpen;

        private long _userOid;
        private string _tabId = "";
        private bool _isDisposed;
        #endregion (Fields)

        protected string _userName => _appState?.CurrentLoggedInUser?.DisplayName ?? "";

        #region Lifecycle
        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if(!firstRender) return;

            var oUserAndTab = await _appStateManager.GetUserAndTabAsync(JS, _sessionStorage);
            if(oUserAndTab is null) return;   // nobody signed in - the page handles the redirect

            (_userOid, _tabId) = oUserAndTab.Value;
            _appState = await _appStateManager.GetOrCreateAppStateAsync(_userOid, _tabId);

            // Subscribe before checking IsAuthenticated: after a server restart the page restores
            // the user from localStorage a moment later, and that arrives as a state change.
            _appState.OnStateChanged += OnAppStateChanged;
            _isHydrated = _appState.IsAuthenticated;

            await InvokeAsync(StateHasChanged);
        }

        protected virtual void OnAppStateChanged() {
            _isHydrated = _appState?.IsAuthenticated == true;
            if(!_isHydrated) _isOpen = false;
            InvokeAsync(StateHasChanged);
        }

        public virtual void Dispose() {
            if(_isDisposed) return;
            _isDisposed = true;
            if(_appState != null) _appState.OnStateChanged -= OnAppStateChanged;
        }
        #endregion (Lifecycle)

        #region Menu
        protected void ToggleOpen() => _isOpen = !_isOpen;

        protected void Close() => _isOpen = false;

        protected void HandleKeyDown(KeyboardEventArgs toArgs) {
            if(toArgs.Key == "Escape") Close();
        }

        /// <summary>
        /// Opens the shared theme picker. It previews each theme live and puts the original back
        /// on Cancel, so only an OK needs handling here.
        /// </summary>
        protected async Task OnThemeAsync() {
            Close();
            if(_appState == null) return;

            var oParameters = new DialogParameters<ModThemePreferences> {
                { toDialog => toDialog.CurrentTheme, _appState.CurrentThemeName }
            };

            IDialogReference oDialog = await _fsModalService.ShowAsync<ModThemePreferences>("", oParameters);
            DialogResult? oResult = await oDialog.Result;

            if(oResult is { Canceled: false, Data: string sThemeName }) {
                // CurrentThemeName is what the base page re-applies on every page load, so the
                // pick survives navigation. It is NOT yet saved anywhere durable: a new sign-in
                // starts on the default theme again until the schema has a user-preferences
                // table to persist it in.
                _appState.CurrentThemeName = sThemeName;
            }
        }

        /// <summary>
        /// Signs out of this tab: the auth cookie, the localStorage snapshot and the server-side
        /// AppState all go, then a full reload lands on the sign-in page.
        /// </summary>
        protected async Task OnLogoutAsync() {
            Close();

            try {
                // The cookie first: it is the one step that depends on the network. If it fails,
                // nothing else has been touched and the user is still fully signed in.
                bool bCookieCleared = await JS.InvokeAsync<bool>("cookieBridge.logout");
                if(!bCookieCleared) {
                    throw new InvalidOperationException("The server did not clear the sign-in cookie.");
                }

                if(_tabId.Length > 0) {
                    await _appStateManager.ClearStorageAsync(_userOid, _tabId, _localStorage);
                }

                if(_appState != null) {
                    await _appState.ClearAsync();
                }

                if(_tabId.Length > 0) {
                    _appStateManager.RemoveAppState(_userOid, _tabId);
                }

                await _authService.LogoutAsync();

                _nav.NavigateTo("/", forceLoad: true);
            } catch(Exception oEx) {
                // Stay on the page and say why - a silent failure would look like a dead button.
                _toasterService.Add($"Logout did not complete: {oEx.Message}", Severity.Error);
            }
        }
        #endregion (Menu)
    }
}
