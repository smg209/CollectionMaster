using CM.Shared;
using FSAuth.Core;
using Microsoft.Extensions.Logging.Abstractions;

namespace CM.UnitTests {

    public class AppStateManagerTests {

        private static AppStateManager CreateManager() => new(NullLogger<AppStateManager>.Instance);

        [Fact]
        public async Task GetOrCreate_SameUserAndTab_ReturnsTheSameInstance() {
            AppStateManager oManager = CreateManager();

            AppState_CM oFirst = await oManager.GetOrCreateAppStateAsync(7, "tab-a");
            AppState_CM oSecond = await oManager.GetOrCreateAppStateAsync(7, "tab-a");

            Assert.Same(oFirst, oSecond);
            Assert.Equal(7, oFirst.UserOid);
        }

        [Fact]
        public async Task GetOrCreate_SameUserDifferentTab_ReturnsIndependentInstances() {
            AppStateManager oManager = CreateManager();

            AppState_CM oTabA = await oManager.GetOrCreateAppStateAsync(7, "tab-a");
            AppState_CM oTabB = await oManager.GetOrCreateAppStateAsync(7, "tab-b");

            Assert.NotSame(oTabA, oTabB);
        }

        [Fact]
        public async Task GetAppState_BeforeCreate_ReturnsNull() {
            AppStateManager oManager = CreateManager();

            Assert.Null(await oManager.GetAppStateAsync(7, "tab-a"));
        }

        [Fact]
        public async Task RemoveAppState_DropsOnlyThatTab() {
            AppStateManager oManager = CreateManager();
            await oManager.GetOrCreateAppStateAsync(7, "tab-a");
            AppState_CM oTabB = await oManager.GetOrCreateAppStateAsync(7, "tab-b");

            oManager.RemoveAppState(7, "tab-a");

            Assert.Null(await oManager.GetAppStateAsync(7, "tab-a"));
            Assert.Same(oTabB, await oManager.GetAppStateAsync(7, "tab-b"));
        }
    }

    public class AppStateManager_SignInDisabledTests {

        [Fact]
        public async Task GetOrCreate_WhenSignInIsNotRequired_StartsAsTheLocalUser() {
            CMSettings.RequireSignIn = false;
            try {
                var oManager = new AppStateManager(NullLogger<AppStateManager>.Instance);

                AppState_CM oAppState = await oManager.GetOrCreateAppStateAsync(CMConstants.LocalUserOid, "tab-a");

                Assert.True(oAppState.IsAuthenticated);
                Assert.Equal("Local User", oAppState.CurrentLoggedInUser?.DisplayName);
                Assert.Equal(CMConstants.DefaultThemeName, oAppState.CurrentThemeName);
                Assert.Null(oAppState.CollectorOid);
            } finally {
                CMSettings.RequireSignIn = true;
            }
        }
    }

    public class AppState_CMTests {

        private static UserAuth_DTO CreateSignedInUser() {
            var oUser = new UserAuth_DTO { Oid = 7, FirstName = "Test", LastName = "Collector" };
            oUser.UpdateToken("token", TimeSpan.FromHours(1));
            return oUser;
        }

        [Fact]
        public void NewAppState_IsNotAuthenticated() {
            var oAppState = new AppState_CM();

            Assert.False(oAppState.IsAuthenticated);
            Assert.Null(oAppState.CurrentLoggedInUser);
        }

        [Fact]
        public void SettingCurrentLoggedInUser_Authenticates_AndPicksTheDefaultTheme() {
            var oAppState = new AppState_CM();
            int iNotifications = 0;
            oAppState.OnStateChanged += () => iNotifications++;

            oAppState.CurrentLoggedInUser = CreateSignedInUser();

            Assert.True(oAppState.IsAuthenticated);
            Assert.Equal(CMConstants.DefaultThemeName, oAppState.CurrentThemeName);
            Assert.Equal(1, iNotifications);
        }

        [Fact]
        public void SettingCurrentLoggedInUser_KeepsAThemeAlreadyChosen() {
            var oAppState = new AppState_CM { CurrentThemeName = "Dark" };

            oAppState.CurrentLoggedInUser = CreateSignedInUser();

            Assert.Equal("Dark", oAppState.CurrentThemeName);
        }

        [Fact]
        public async Task ClearAsync_RemovesTheUserAndTheCollector() {
            var oAppState = new AppState_CM { CurrentLoggedInUser = CreateSignedInUser() };
            await oAppState.InitializeAsync(42);

            await oAppState.ClearAsync();

            Assert.False(oAppState.IsAuthenticated);
            Assert.Null(oAppState.CollectorOid);
        }
    }
}
