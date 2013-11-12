using MemBus.Publishing;
using MemBus.Setup;
using MemBus.Subscribing;

namespace MemBus.Configurators
{
    /// <summary>
    /// A fire and forget publisher is used that publishes messages in parallel. With this setup <see cref="IBus.Publish"/> does NOT block.
    /// </summary>
    public class Fast : ISetup<IConfigurableBus>
    {

        /// <summary>
        /// <see cref="ISetup{T}.Accept"/>
        /// </summary>
        public virtual void Accept(IConfigurableBus setup)
        {
            setup.ConfigurePublishing(p => p.DefaultPublishPipeline(new FireAndForgetPublisher()));
            setup.AddResolver(new StandardResolver());
            setup.ConfigureSubscribing(cs => cs.ApplyOnNewSubscription(new ShapeToDispose()));
        }
    }
}