using Blazored.LocalStorage;
using FSAuth.Core;

namespace CM.Shared {

    /// <summary>
    /// CollectionMaster's AppState manager contract: the shared tab-scoped members from
    /// FSCommon.IAppStateManager&lt;TAppState&gt; plus the localStorage snapshot helpers the
    /// sign-in and sign-out flows use.
    /// </summary>
    public interface IAppStateManager : FSCommon.IAppStateManager<AppState_CM> {
        Task RestoreFromStorageAsync(long tlUserOid, string tsTabId, ILocalStorageService toStorage, bool tbIsAuthenticated);
        Task SaveToStorageAsync(long tlUserOid, string tsTabId, UserAuth_DTO toUser, ILocalStorageService toStorage);
        Task ClearStorageAsync(long tlUserOid, string tsTabId, ILocalStorageService toStorage);
        Task<bool> IsUserAuthenticatedAsync(long tlUserOid, string tsTabId, ILocalStorageService toStorage);
    }
}
