using System;
using Mirror;

namespace MessageService.Networking
{
    /// <summary>
    /// NetworkManager that exposes Mirror lifecycle callbacks as C# events so
    /// plain (non-MonoBehaviour) services can react to them without touching
    /// Mirror internals. Use it on the scene instead of the stock
    /// NetworkManager and subscribe to the events from DI-managed services.
    /// </summary>
    public class MessagingNetworkManager : NetworkManager
    {
        public event Action ServerStarted;
        public event Action ServerStopped;
        public event Action<NetworkConnectionToClient> ServerClientDisconnected;
        public event Action ClientConnected;
        public event Action ClientDisconnected;

        public override void OnStartServer() => ServerStarted?.Invoke();

        public override void OnStopServer() => ServerStopped?.Invoke();

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            ServerClientDisconnected?.Invoke(conn);
            base.OnServerDisconnect(conn);
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            ClientConnected?.Invoke();
        }

        public override void OnClientDisconnect() => ClientDisconnected?.Invoke();
    }
}
