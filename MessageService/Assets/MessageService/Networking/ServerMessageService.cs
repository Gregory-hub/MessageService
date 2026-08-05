using System;
using System.Collections.Generic;
using MessageService.Messages;
using Mirror;
using VContainer.Unity;

namespace MessageService.Networking
{
    /// <summary>
    /// Default IServerMessageService implementation. Handles subscribe and
    /// unsubscribe requests from clients, keeps a message id to connections
    /// map and cleans it up on client disconnect and server stop.
    /// Registered in the DI container as a VContainer entry point.
    /// </summary>
    public sealed class ServerMessageService : IServerMessageService, IStartable, IDisposable
    {
        readonly MessagingNetworkManager networkManager;
        readonly Dictionary<ushort, HashSet<NetworkConnectionToClient>> subscribers =
            new Dictionary<ushort, HashSet<NetworkConnectionToClient>>();

        public event Action<NetworkConnectionToClient, ushort> ClientSubscribed;

        public ServerMessageService(MessagingNetworkManager networkManager)
        {
            this.networkManager = networkManager;
        }

        public void Start()
        {
            networkManager.ServerStarted += OnServerStarted;
            networkManager.ServerStopped += OnServerStopped;
            networkManager.ServerClientDisconnected += OnClientDisconnected;
        }

        public void Dispose()
        {
            networkManager.ServerStarted -= OnServerStarted;
            networkManager.ServerStopped -= OnServerStopped;
            networkManager.ServerClientDisconnected -= OnClientDisconnected;
        }

        public void SendToSubscribers<T>(T message, int channelId = Channels.Reliable) where T : struct, NetworkMessage
        {
            if (!subscribers.TryGetValue(NetworkMessageId<T>.Id, out HashSet<NetworkConnectionToClient> connections))
                return;

            foreach (NetworkConnectionToClient connection in connections)
                connection.Send(message, channelId);
        }

        public bool TrySendToClient<T>(NetworkConnectionToClient connection, T message, int channelId = Channels.Reliable) where T : struct, NetworkMessage
        {
            if (!subscribers.TryGetValue(NetworkMessageId<T>.Id, out HashSet<NetworkConnectionToClient> connections))
                return false;
            if (!connections.Contains(connection))
                return false;

            connection.Send(message, channelId);
            return true;
        }

        // Mirror clears its own handlers on shutdown, so handlers must be
        // registered again on every server start.
        void OnServerStarted()
        {
            NetworkServer.RegisterHandler<SubscribeRequest>(OnSubscribeRequest);
            NetworkServer.RegisterHandler<UnsubscribeRequest>(OnUnsubscribeRequest);
        }

        void OnServerStopped() => subscribers.Clear();

        void OnClientDisconnected(NetworkConnectionToClient connection)
        {
            foreach (HashSet<NetworkConnectionToClient> connections in subscribers.Values)
                connections.Remove(connection);
        }

        void OnSubscribeRequest(NetworkConnectionToClient connection, SubscribeRequest request)
        {
            if (!subscribers.TryGetValue(request.MessageId, out HashSet<NetworkConnectionToClient> connections))
            {
                connections = new HashSet<NetworkConnectionToClient>();
                subscribers[request.MessageId] = connections;
            }

            if (connections.Add(connection))
                ClientSubscribed?.Invoke(connection, request.MessageId);
        }

        void OnUnsubscribeRequest(NetworkConnectionToClient connection, UnsubscribeRequest request)
        {
            if (subscribers.TryGetValue(request.MessageId, out HashSet<NetworkConnectionToClient> connections))
                connections.Remove(connection);
        }
    }
}
