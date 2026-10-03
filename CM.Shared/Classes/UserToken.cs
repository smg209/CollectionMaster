namespace CM.Shared {

    /// <summary>Request body of POST /auth/exchange (see Program.cs and cookieBridge.js).</summary>
    public class UserToken {
        public long UserOid { get; set; }
    }
}
