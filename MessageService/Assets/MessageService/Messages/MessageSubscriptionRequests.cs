using Mirror;

namespace MessageService.Messages
{
    /// <summary>
    /// Client-to-server request to start receiving messages of the type
    /// identified by the Mirror message id (NetworkMessageId&lt;T&gt;.Id).
    /// </summary>
    public struct SubscribeRequest : NetworkMessage
    {
        public ushort MessageId;
    }

    /// <summary>
    /// Client-to-server request to stop receiving messages of the type
    /// identified by the Mirror message id (NetworkMessageId&lt;T&gt;.Id).
    /// </summary>
    public struct UnsubscribeRequest : NetworkMessage
    {
        public ushort MessageId;
    }
}
