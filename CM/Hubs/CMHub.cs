using Microsoft.AspNetCore.SignalR;

namespace CM {

    /// <summary>
    /// CollectionMaster's real-time channel, mapped at CMConstants.HubPath in Program.cs.
    /// Deliberately empty: no feature pushes live updates yet. Add server-to-client messages
    /// here as they are designed (e.g. a price update for a tracked collectible).
    ///
    /// Before the first tenant-scoped message is sent, connections must be tied to the signed-in
    /// user (the "uid" cookie the /auth endpoints issue) and grouped by CollectorOid - a hub that
    /// broadcasts to all clients would leak one collector's data to another.
    /// </summary>
    public class CMHub : Hub {
    }
}
