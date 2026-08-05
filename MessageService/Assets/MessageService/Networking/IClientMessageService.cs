using System;
using Mirror;

namespace MessageService.Networking
{
    /// <summary>
    /// Client-side messaging service. Subscribe registers a local Mirror
    /// handler for T and notifies the server, so the server only sends
    /// messages this client explicitly asked for. Call while connected.
    /// </summary>
    public interface IClientMessageService
    {
        void Subscribe<T>(Action<T> handler) where T : struct, NetworkMessage;

        void Unsubscribe<T>() where T : struct, NetworkMessage;
    }
}
