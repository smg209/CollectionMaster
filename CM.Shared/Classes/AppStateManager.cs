using Blazored.LocalStorage;
using Blazored.SessionStorage;
using FSAuth.Core;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace CM.Shared {

    /// <summary>
    /// Singleton that owns every live AppState_CM, one per (userOid, tabId): the same user in two
    /// browser tabs has two independent instances.
    /// </summary>
    public class AppStateManager : IAppStateManager {

        #region Fields
        private const string TabIdStorageKey = "tab_id";

        private readonly ConcurrentDictionary<string, AppState_CM> _appStates = new();
        private readonly ILogger<AppStateManager> _logger;

        // One lock per circuit (IJSRuntime is scoped to the circuit), guarding the
        // read-generate-write of sessionStorage's tab_id in GetUserAndTabAsync. A page and the
        // layout widgets around it all resolve their AppState on the same first render; without a
        // lock each would read an empty tab_id and mint its own, splitting one browser tab across
        // several AppState instances. Per circuit rather than one global lock so a slow or stuck
        // browser can only ever hold up itself.
        private readonly ConditionalWeakTable<IJSRuntime, SemaphoreSlim> _tabIdLocks = new();
        #endregion (Fields)

        public AppStateManager(ILogger<AppStateManager> toLogger) {
            _logger = toLogger;
        }

        #region Keys
        private static string GetAppStateKey(long tlUserOid, string tsTabId) => $"{tlUserOid}_{tsTabId}";

        // localStorage key of the signed-in user's snapshot for one tab.
        private static string GetStorageKey(long tlUserOid, string tsTabId) => $"appstate_{tlUserOid}_{tsTabId}";
        #endregion (Keys)

        #region Tab-scoped AppState
        public Task<AppState_CM> GetOrCreateAppStateAsync(long tlUserOid, string tsTabId) {
            string sKey = GetAppStateKey(tlUserOid, tsTabId);

            AppState_CM oAppState = _appStates.GetOrAdd(sKey, _ => {
                _logger.LogInformation("Creating AppState_CM for user {UserOid}, tab {TabId}", tlUserOid, tsTabId);
                var oNew = new AppState_CM();
                oNew.AttachToUser(tlUserOid);

                // Sign-in switched off: there is no sign-in flow to attach a user, so every new
                // AppState starts with the built-in local user already on it.
                if(!CMSettings.RequireSignIn) {
                    oNew.CurrentLoggedInUser = CreateLocalUser();
                }

                return oNew;
            });

            return Task.FromResult(oAppState);
        }

        public Task<AppState_CM?> GetAppStateAsync(long tlUserOid, string tsTabId) {
            _appStates.TryGetValue(GetAppStateKey(tlUserOid, tsTabId), out var oAppState);
            return Task.FromResult(oAppState);
        }

        public void RemoveAppState(long tlUserOid, string tsTabId) {
            if(_appStates.TryRemove(GetAppStateKey(tlUserOid, tsTabId), out var oAppState)) {
                _logger.LogInformation("Removed AppState_CM for user {UserOid}, tab {TabId}", tlUserOid, tsTabId);
                oAppState.Dispose();
            }
        }

        /// <summary>
        /// Resolves the cookie-verified user and this browser tab's id. Returns null when nobody
        /// is signed in. With sign-in switched off (CMSettings.RequireSignIn) it never returns
        /// null: every tab is the built-in local user.
        /// Needs JS interop, so call it from OnAfterRenderAsync, never earlier.
        /// </summary>
        public async Task<(long userOid, string tabId)?> GetUserAndTabAsync(IJSRuntime toJs, ISessionStorageService toSession) {
            long lUserOid = CMConstants.LocalUserOid;

            if(CMSettings.RequireSignIn) {
                WhoAmIResult? oWho = await toJs.InvokeAsync<WhoAmIResult?>("cookieBridge.whoami");
                if(oWho is null || oWho.UserOid <= 0) return null;
                lUserOid = oWho.UserOid;
            }

            SemaphoreSlim oLock = _tabIdLocks.GetValue(toJs, _ => new SemaphoreSlim(1, 1));
            await oLock.WaitAsync();
            try {
                string? sTabId = await toSession.GetItemAsStringAsync(TabIdStorageKey);
                if(string.IsNullOrWhiteSpace(sTabId)) {
                    sTabId = Guid.NewGuid().ToString("N")[..8];
                    await toSession.SetItemAsStringAsync(TabIdStorageKey, sTabId);
                }
                return (lUserOid, sTabId);
            } finally {
                oLock.Release();
            }
        }

        /// <summary>
        /// The stand-in user every tab gets while sign-in is switched off. It carries a token
        /// only because UserAuth_DTO.IsAuthenticated is defined by one; nothing validates it.
        /// TenantOid stays 0, which AppState_CM.EnsureCollectorAsync maps to the one local Collector.
        /// </summary>
        public static UserAuth_DTO CreateLocalUser() {
            var oUser = new UserAuth_DTO { Oid = CMConstants.LocalUserOid, FirstName = "Local", LastName = "User" };
            oUser.UpdateToken("sign-in-disabled", TimeSpan.FromDays(3650));
            return oUser;
        }

        /// <summary>
        /// Reports how many instances are held. Nothing is evicted yet: an instance leaves only
        /// through RemoveAppState (sign-out). Idle eviction needs a last-used time per instance
        /// and something to call this on a schedule - add both before the app carries real load.
        /// </summary>
        public void CleanupInactiveAppStates() {
            _logger.LogDebug("AppState_CM cleanup check - {Count} instance(s) held", _appStates.Count);
        }
        #endregion (Tab-scoped AppState)

        #region localStorage Snapshot
        public async Task SaveToStorageAsync(long tlUserOid, string tsTabId, UserAuth_DTO toUser, ILocalStorageService toStorage) {
            try {
                await toStorage.SetItemAsync(GetStorageKey(tlUserOid, tsTabId), toUser);
            } catch(Exception oEx) {
                _logger.LogError(oEx, "Failed to save the user snapshot for user {UserOid}, tab {TabId}", tlUserOid, tsTabId);
            }
        }

        public async Task RestoreFromStorageAsync(long tlUserOid, string tsTabId, ILocalStorageService toStorage, bool tbIsAuthenticated) {
            if(!tbIsAuthenticated) {
                _logger.LogWarning("Refused to restore AppState_CM for unauthenticated user {UserOid}, tab {TabId}", tlUserOid, tsTabId);
                return;
            }

            AppState_CM oAppState = await GetOrCreateAppStateAsync(tlUserOid, tsTabId);
            if(!await TryRestoreFromStorageAsync(tlUserOid, tsTabId, oAppState, toStorage)) {
                // Missing or expired - drop it so a stale snapshot is never read again.
                await ClearStorageAsync(tlUserOid, tsTabId, toStorage);
            }
        }

        /// <summary>
        /// Called by FSCommon.BaseAppStatePage when a cookie-verified user/tab pair has an AppState
        /// with no user attached - typically a server restart wiped the in-memory instances while
        /// the browser's cookie is still valid. Re-attaches the user from the snapshot the sign-in
        /// flow saved. Returns false when there is nothing usable (missing, or its token expired).
        /// </summary>
        public async Task<bool> TryRestoreFromStorageAsync(long tlUserOid, string tsTabId, AppState_CM toAppState, ILocalStorageService toLocalStorage) {
            try {
                UserAuth_DTO? oUser = await toLocalStorage.GetItemAsync<UserAuth_DTO>(GetStorageKey(tlUserOid, tsTabId));

                // The snapshot is written by the browser, so it is only trusted for the user the
                // server-issued cookie already named.
                if(oUser?.IsAuthenticated == true && oUser.Oid == tlUserOid) {
                    toAppState.CurrentLoggedInUser = oUser;
                    _logger.LogInformation("Restored AppState_CM from storage for user {UserOid}, tab {TabId}", tlUserOid, tsTabId);
                    return true;
                }

                return false;
            } catch(Exception oEx) {
                _logger.LogError(oEx, "Failed to restore AppState_CM from storage for user {UserOid}, tab {TabId}", tlUserOid, tsTabId);
                return false;
            }
        }

        public async Task ClearStorageAsync(long tlUserOid, string tsTabId, ILocalStorageService toStorage) {
            try {
                await toStorage.RemoveItemAsync(GetStorageKey(tlUserOid, tsTabId));
            } catch(Exception oEx) {
                _logger.LogError(oEx, "Failed to clear the user snapshot for user {UserOid}, tab {TabId}", tlUserOid, tsTabId);
            }
        }

        public Task<bool> IsUserAuthenticatedAsync(long tlUserOid, string tsTabId, ILocalStorageService toStorage) {
            bool bIsAuthenticated = _appStates.TryGetValue(GetAppStateKey(tlUserOid, tsTabId), out var oAppState)
                && oAppState.IsAuthenticated;
            return Task.FromResult(bIsAuthenticated);
        }
        #endregion (localStorage Snapshot)

        // Shape of cookieBridge.whoami()'s result ({ userOid }). JS interop matches property names
        // case-insensitively.
        private sealed class WhoAmIResult {
            public long UserOid { get; set; }
        }
    }
}
