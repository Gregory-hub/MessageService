using System;
using System.Collections.Generic;
using MessageService.Messages;
using Mirror;
using VContainer.Unity;

namespace MessageService.Networking
{
    /// <summary>
    /// Default IClientMessageService implementation. Registers the local
    /// Mirror handler first and only then sends the subscribe request, which
    /// guarantees the handler exists before the server may send the message.
    /// Registered in the DI container as a VContainer entry point.
    /// </summary>
    public sealed class ClientMessageService : IClientMessageService, IStartable, IDisposable
    {
        readonly MessagingNetworkManager networkManager;
        readonly HashSet<ushort> subscribedIds = new HashSet<ushort>();

        public ClientMessageService(MessagingNetworkManager networkManager)
        {
            this.networkManager = networkManager;
        }

        public void Start() => networkManager.ClientDisconnected += OnClientDisconnected;

        public void Dispose() => networkManager.ClientDisconnected -= OnClientDisconnected;

        public void Subscribe<T>(Action<T> handler) where T : struct, NetworkMessage
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            if (!NetworkClient.isConnected)
                throw new InvalidOperationException($"Cannot subscribe to {typeof(T).Name}: client is not connected.");

            ushort messageId = NetworkMessageId<T>.Id;
            if (subscribedIds.Add(messageId))
            {
                NetworkClient.RegisterHandler(handler);
                NetworkClient.Send(new SubscribeRequest { MessageId = messageId });
            }
            else
            {
                // already subscribed on the server, only swap the local handler
                NetworkClient.ReplaceHandler(handler);
            }
        }

        public void Unsubscribe<T>() where T : struct, NetworkMessage
        {
            ushort messageId = NetworkMessageId<T>.Id;
            if (!subscribedIds.Remove(messageId))
                return;

            NetworkClient.UnregisterHandler<T>();
            if (NetworkClient.isConnected)
                NetworkClient.Send(new UnsubscribeRequest { MessageId = messageId });
        }

        // Mirror clears its own handlers on shutdown; mirror that locally so
        // a reconnected client can subscribe again.
        void OnClientDisconnected() => subscribedIds.Clear();
    }
}
