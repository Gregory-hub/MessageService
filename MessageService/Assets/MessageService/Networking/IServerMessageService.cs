using System;
using Mirror;

namespace MessageService.Networking
{
    /// <summary>
    /// Server-side messaging service. Tracks which clients subscribed to
    /// which message types and sends messages only to those clients, so
    /// nobody receives a message type they have no handler for.
    /// </summary>
    public interface IServerMessageService
    {
        /// <summary>
        /// Raised when a client subscribes to a message type. The ushort is
        /// the Mirror message id, compare it with NetworkMessageId&lt;T&gt;.Id.
        /// </summary>
        event Action<NetworkConnectionToClient, ushort> ClientSubscribed;

        /// <summary>Sends the message to every client subscribed to T.</summary>
        void SendToSubscribers<T>(T message, int channelId = Channels.Reliable) where T : struct, NetworkMessage;

        /// <summary>Sends the message to one client only if it is subscribed to T.</summary>
        bool TrySendToClient<T>(NetworkConnectionToClient connection, T message, int channelId = Channels.Reliable) where T : struct, NetworkMessage;
    }
}
