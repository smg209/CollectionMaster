namespace CM.Shared {

    /// <summary>
    /// Switches read once from configuration at startup (Program.cs). Unlike CMConstants these
    /// can differ between environments.
    /// </summary>
    public static class CMSettings {

        /// <summary>
        /// appsettings "Authentication:RequireSignIn". True is the real behaviour: nobody gets
        /// past the sign-in page without an FSAuthentication login.
        ///
        /// False removes sign-in entirely: every browser tab is handed the same built-in local
        /// user (AppStateManager.CreateLocalUser), "/" goes straight to the home page and Logout
        /// is hidden. That user has no Authentication tenant; it owns the one local Collector
        /// (TenantOid 0), so the collection pages still work. Never run a reachable deployment
        /// with this off: there is no access control at all and everyone shares that collection.
        /// </summary>
        public static bool RequireSignIn { get; set; } = true;
    }
}
