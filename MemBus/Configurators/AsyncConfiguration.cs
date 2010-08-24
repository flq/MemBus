using MemBus.Messages;
using MemBus.Publishing;
using MemBus.Setup;
using MemBus.Subscribing;

namespace MemBus.Configurators
{
    /// <summary>
    /// A parallel publisher is used that publishes messages in parallel. With this setup <see cref="IBus.Publish"/> does NOT block.
    /// Exceptions will become available once all subscriptions are done processing the message as <see cref="ExceptionOccurred"/> message.
    /// </summary>
    public class AsyncConfiguration : ISetup<IConfigurableBus>
    {
        public virtual void Accept(IConfigurableBus setup)
        {
            setup.ConfigurePublishing(p => p.DefaultPublishPipeline(new ParallelNonBlockingPublisher()));
            setup.AddResolver(new TableBasedResolver());
            setup.ConfigureSubscribing(cs => cs.ApplyOnNewSubscription(new ShapeToDispose()));
        }
    }
}