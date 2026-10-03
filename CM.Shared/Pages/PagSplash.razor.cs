using Blazored.LocalStorage;
using Blazored.SessionStorage;
using FSAuth.Core;
using FSCommon;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CM.Shared {

    /// <summary>
    /// The app's front door ("/"): hosts the shared FSCommon.ModSignIn form inline. This is also
    /// where FSCommon.BaseAppStatePage sends anyone who is not signed in, so it deliberately does
    /// NOT inherit BaseAppStatePage - that would redirect to itself.
    /// </summary>
    public class PagSplash_Base : ComponentBase {

        #region Injected Services
        [Inject] protected IJSRuntime JS { get; set; } = default!;
        [Inject] protected ISessionStorageService _sessionStorage { get; set; } = default!;
        [Inject] protected ILocalStorageService _localStorage { get; set; } = default!;
        [Inject] protected IAppStateManager _appStateManager { get; set; } = default!;
        [Inject] protected NavigationManager _nav { get; set; } = default!;
        [Inject] protected ThemeService _themeService { get; set; } = default!;
        #endregion (Injected Services)

        // ModSignIn reports a wrong email/password itself. This is for the step after that:
        // the credentials were accepted but the session could not be started.
        protected string _signInError = "";

        protected override async Task OnAfterRenderAsync(bool firstRender) {
            if(!firstRender) return;

            // No signed-in user means no saved preference - the sign-in form gets the default theme.
            await _themeService.SetThemeAsync(CMConstants.DefaultThemeName);

            // Already signed in on this tab (a refresh, or someone typing "/")? Skip the form.
            var oUserAndTab = await _appStateManager.GetUserAndTabAsync(JS, _sessionStorage);
            if(oUserAndTab is null) return;

            var (lUserOid, sTabId) = oUserAndTab.Value;
            AppState_CM oAppState = await _appStateManager.GetOrCreateAppStateAsync(lUserOid, sTabId);

            bool bIsSignedIn = oAppState.IsAuthenticated
                || await _appStateManager.TryRestoreFromStorageAsync(lUserOid, sTabId, oAppState, _localStorage);

            if(bIsSignedIn) {
                _nav.NavigateTo(CMConstants.HomeRoute);
            }
        }

        /// <summary>
        /// Fired by ModSignIn once FSAuthentication has accepted the credentials. Turns that into a
        /// session: auth cookie, tab-scoped AppState, and the localStorage snapshot that lets the
        /// session survive a server restart.
        /// </summary>
        protected async Task HandleLoginSuccessAsync(FSLoginResponse toResponse) {
            _signInError = "";

            try {
                UserAuth_DTO oUser = toResponse.User
                    ?? throw new InvalidOperationException("The sign-in response did not include a user.");

                // 1) The HttpOnly cookie first - everything after this asks the server who we are.
                bool bCookieSet = await JS.InvokeAsync<bool>("cookieBridge.exchangeUser", oUser.Oid);
                if(!bCookieSet) {
                    throw new InvalidOperationException("The server did not issue the sign-in cookie.");
                }

                // 2) (userOid, tabId) as the server now sees them.
                var oUserAndTab = await _appStateManager.GetUserAndTabAsync(JS, _sessionStorage);
                if(oUserAndTab is null) {
                    throw new InvalidOperationException("The sign-in cookie was not accepted by the server.");
                }

                var (lUserOid, sTabId) = oUserAndTab.Value;

                // 3) Attach the user to this tab's AppState.
                AppState_CM oAppState = await _appStateManager.GetOrCreateAppStateAsync(lUserOid, sTabId);
                oAppState.CurrentLoggedInUser = oUser;

                // 4) Snapshot for recovery after a server restart.
                await _appStateManager.SaveToStorageAsync(lUserOid, sTabId, oUser, _localStorage);

                // 5) Attach this user's Collector (created on the first sign-in).
                await oAppState.EnsureCollectorAsync();

                _nav.NavigateTo(CMConstants.HomeRoute);
            } catch(Exception oEx) {
                _signInError = $"Your email and password were accepted, but the session could not be started: {oEx.Message}";
                StateHasChanged();
            }
        }
    }
}
