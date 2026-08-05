using System;
using MessageService.Messages;
using MessageService.Networking;
using UnityEngine;
using VContainer.Unity;

namespace MessageService.Demo
{
    /// <summary>
    /// Demo client flow: after connecting to the server, subscribes to
    /// HelloMessage and logs the received text to the console.
    /// </summary>
    public sealed class HelloMessageClientDemo : IStartable, IDisposable
    {
        readonly MessagingNetworkManager networkManager;
        readonly IClientMessageService messageService;

        public HelloMessageClientDemo(MessagingNetworkManager networkManager, IClientMessageService messageService)
        {
            this.networkManager = networkManager;
            this.messageService = messageService;
        }

        public void Start() => networkManager.ClientConnected += OnClientConnected;

        public void Dispose() => networkManager.ClientConnected -= OnClientConnected;

        void OnClientConnected() => messageService.Subscribe<HelloMessage>(OnHelloMessage);

        static void OnHelloMessage(HelloMessage message) =>
            Debug.Log($"[Client] Received HelloMessage: {message.Text}");
    }
}
