namespace CM.Shared {

    /// <summary>App-wide names that more than one project must agree on.</summary>
    public static class CMConstants {

        // Passed to IFSAuthenticationService.Authenticate. Sign-in only succeeds for a user mapped
        // to a Tenant row whose AppName is exactly this value (Authentication database).
        public const string AppName = "CollectionMaster";

        // The application database. Must match the ConnectionStrings key in appsettings and the
        // DatabaseName of every generated Record<T> class.
        public const string DatabaseName = "CollectionMaster";

        // Applied until the signed-in user's own preference is known. Must be one of the names
        // registered with FSCommon.ThemeService in Program.cs.
        public const string DefaultThemeName = "Ocean";

        // Where a signed-in user lands, and where the brand link in MainLayout points.
        public const string HomeRoute = "/pagHome";

        // UserOid of the built-in local user every tab gets while CMSettings.RequireSignIn is
        // false. Zero on purpose: it is not a row in Authentication.UserAuth.
        public const long LocalUserOid = 0;

        // The catalog: sets, then the cards of a set, then one card.
        public const string CatalogRoute = "/pagSets";
        public const string SetCardsRoute = "/pagSetCards";
        public const string CardRoute = "/pagCard";
        public const string SearchRoute = "/pagSearch";
        public const string CollectionRoute = "/pagCollection";
        public const string MarketRoute = "/pagMarket";

        // SignalR endpoint - shared so the server mapping and any client connection cannot drift.
        public const string HubPath = "/hubs/cm";
    }
}
