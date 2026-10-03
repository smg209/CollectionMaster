using FSAuth.Core;
using FSDataUtil.Core;

namespace CM.Shared {

    /// <summary>
    /// CollectionMaster's working context for one signed-in user in one browser tab.
    /// Do not redeclare anything BaseAppState already provides (OnStateChanged, ActiveMenuKey,
    /// CurrentThemeName, ...) - a same-named member here would shadow the base one, and the
    /// shared base page reads the base member.
    /// </summary>
    public class AppState_CM : BaseAppState {

        #region Multi-tenancy
        // The tenant in CollectionMaster is the Collector (one account owner), not a company.
        // Every tenant-scoped table carries CollectorOid and every query on one filters by it.
        public Collector? Collector { get; private set; }
        public long? CollectorOid { get; private set; }
        #endregion (Multi-tenancy)

        #region Current User
        public UserAuth_DTO? CurrentLoggedInUser {
            get => _currentIUserDTO as UserAuth_DTO;
            set {
                // _currentIUserDTO is what BaseAppState.IsAuthenticated and TenantOid read.
                _currentIUserDTO = value;

                // The shared base page applies CurrentThemeName on every page load and skips the
                // theme entirely when it is empty, so a signed-in user always needs one.
                if(value != null && string.IsNullOrEmpty(_currentThemeName)) {
                    _currentThemeName = CMConstants.DefaultThemeName;
                }

                NotifyStateChanged();
            }
        }
        #endregion (Current User)

        #region Initialization
        /// <summary>
        /// Attaches the signed-in user's Collector, creating it on the first sign-in. Every
        /// Authentication tenant mapped to CollectionMaster is one collector, so the Collector
        /// is found by TenantOid. Does nothing when a Collector is already attached.
        /// </summary>
        public async Task EnsureCollectorAsync() {
            if(CollectorOid != null || !IsAuthenticated) return;

            // With sign-in switched off the built-in local user has no tenant (TenantOid 0). It
            // still gets a Collector - "TenantOid 0" is the one local collection of this machine -
            // so the collection pages work without an account.
            long lTenantOid = TenantOid;
            if(lTenantOid < 0 || (lTenantOid == 0 && CMSettings.RequireSignIn)) return;

            Collector? oCollector = await Collector.FirstAsync("WHERE TenantOid = @0", lTenantOid);

            if(oCollector == null) {
                oCollector = new Collector();
                oCollector.TenantOid = lTenantOid;
                oCollector.DisplayName = CurrentLoggedInUser?.DisplayName ?? "";
                oCollector.IsActive = true;
                oCollector.CreatedOn = DateTime.Now;
                oCollector.UserAuthOid_CreatedBy = CurrentLoggedInUser?.Oid > 0 ? CurrentLoggedInUser?.Oid : null;

                try {
                    await oCollector.SaveAsync();
                } catch(Exception) {
                    // Another tab of the same user created it a moment earlier (TenantOid is unique).
                    oCollector = await Collector.FirstAsync("WHERE TenantOid = @0", lTenantOid);
                    if(oCollector == null) throw;
                }
            }

            Collector = oCollector;
            await InitializeAsync(Convert.ToInt64(oCollector.Oid));
        }

        /// <summary>Attaches this tab's state to its Collector and starts the optional preload.</summary>
        public async Task InitializeAsync(long tlCollectorOid) {
            CollectorOid = tlCollectorOid;
            BeginOptionalPreload();
            NotifyStateChanged();
            await Task.CompletedTask;
        }

        protected override async Task PreloadInitialStateAsync() {
            // Load data the app always needs right after sign-in (lookups, the collector's
            // default collection, ...). Called automatically when LoadMode is PreloadInitialState.
            await Task.CompletedTask;
        }

        public override async Task ClearAsync() {
            Collector = null;
            CollectorOid = null;
            await base.ClearAsync();   // clears the user and fires NotifyStateChanged
        }
        #endregion (Initialization)
    }
}
