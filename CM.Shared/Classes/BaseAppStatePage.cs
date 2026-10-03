namespace CM.Shared {

    /// <summary>
    /// CollectionMaster's typed alias of FSCommon.BaseAppStatePage&lt;TAppState&gt;. Every
    /// authenticated page inherits this; all of the AppState loading, auth redirect and theme
    /// logic lives in the shared generic base.
    ///
    /// What this adds: once AppState is ready it makes sure the signed-in user's Collector is
    /// attached, then calls LoadPageDataAsync() - the one place a page loads its own data.
    ///
    /// Blazor keeps the same page instance when only a route or query parameter changes (one
    /// card to another card). A page that shows something named by its parameters overrides
    /// PageKey, and its data is loaded again whenever that key changes.
    /// </summary>
    public abstract class BaseAppStatePage : FSCommon.BaseAppStatePage<AppState_CM> {

        /// <summary>True once LoadPageDataAsync has finished without an error.</summary>
        protected bool _isPageDataLoaded;

        protected override async Task OnAfterRenderAsync(bool firstRender) {
            await base.OnAfterRenderAsync(firstRender);

            // The base returns without loading AppState when it has redirected to sign-in.
            if(firstRender && _isAppStateLoaded) {
                await RunPageLoadAsync();
            }
        }

        /// <summary>
        /// What the page is showing, as text built from its parameters ("card 4"). Empty for a
        /// page that always shows the same thing.
        /// </summary>
        protected virtual string PageKey => "";

        private string _loadedPageKey = "";

        protected override async Task OnParametersSetAsync() {
            await base.OnParametersSetAsync();

            // Before the first render AppState is not loaded yet; OnAfterRenderAsync does that load.
            if(_isAppStateLoaded && PageKey != _loadedPageKey) {
                _isPageDataLoaded = false;
                await RunPageLoadAsync();
            }
        }

        /// <summary>Override to load the page's data. AppState and the Collector are ready when this runs.</summary>
        protected virtual Task LoadPageDataAsync() => Task.CompletedTask;

        /// <summary>Runs LoadPageDataAsync and reports a failure through _loadError instead of breaking the circuit.</summary>
        protected async Task RunPageLoadAsync() {
            _loadedPageKey = PageKey;

            try {
                _loadError = "";
                await GetAppState().EnsureCollectorAsync();
                await LoadPageDataAsync();
                _isPageDataLoaded = true;
            } catch(Exception oEx) {
                _isPageDataLoaded = false;
                _loadError = oEx.Message;
            }

            await InvokeAsync(StateHasChanged);
        }
    }
}
