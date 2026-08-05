using MessageService.Demo;
using MessageService.Networking;
using VContainer;
using VContainer.Unity;

namespace MessageService.DI
{
    /// <summary>
    /// Composition root. Registers the messaging services and the demo flows
    /// in the VContainer DI container. Lives on a GameObject in the scene.
    /// </summary>
    public sealed class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<MessagingNetworkManager>();

            // RegisterEntryPoint exposes implemented interfaces, so the
            // services are also resolvable as I(Client|Server)MessageService.
            builder.RegisterEntryPoint<ServerMessageService>();
            builder.RegisterEntryPoint<ClientMessageService>();
            builder.RegisterEntryPoint<HelloMessageServerDemo>();
            builder.RegisterEntryPoint<HelloMessageClientDemo>();
        }
    }
}
