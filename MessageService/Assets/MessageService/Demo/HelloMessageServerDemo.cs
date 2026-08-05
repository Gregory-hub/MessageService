using System;
using MessageService.Messages;
using MessageService.Networking;
using Mirror;
using UnityEngine;
using VContainer.Unity;

namespace MessageService.Demo
{
    /// <summary>
    /// Demo server flow: when a client subscribes to HelloMessage, sends it
    /// "Hello Client!".
    /// </summary>
    public sealed class HelloMessageServerDemo : IStartable, IDisposable
    {
        readonly IServerMessageService messageService;

        public HelloMessageServerDemo(IServerMessageService messageService)
        {
            this.messageService = messageService;
        }

        public void Start() => messageService.ClientSubscribed += OnClientSubscribed;

        public void Dispose() => messageService.ClientSubscribed -= OnClientSubscribed;

        void OnClientSubscribed(NetworkConnectionToClient connection, ushort messageId)
        {
            if (messageId != NetworkMessageId<HelloMessage>.Id)
                return;

            Debug.Log($"[Server] Connection {connection.connectionId} subscribed to HelloMessage, greeting it.");
            messageService.TrySendToClient(connection, new HelloMessage { Text = "Hello Client!" });
        }
    }
}
