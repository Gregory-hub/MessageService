using Mirror;

namespace MessageService.Messages
{
    /// <summary>
    /// Greeting message sent by the server to clients subscribed to it.
    /// </summary>
    public struct HelloMessage : NetworkMessage
    {
        public string Text;
    }
}
