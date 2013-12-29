using System.Threading.Tasks;
using MemBus.Messages;
using MemBus.Publishing;
using MemBus.Setup;
using MemBus.Subscribing;

namespace MemBus.Configurators
{
    /// <summary>
    /// Setup the Bus with this on the UI thread (Important!).
    /// A parallel publisher is used that publishes messages in parallel but blocks until all message processing is done.
    /// Exceptions will not stop the publishing and become available on the bus as <see cref="ExceptionOccurred"/> message.
    /// The setup allows you to call <see cref="SubscriptionCustomizer{M}.DispatchOnUiThread"/> when doing a subscription.
    /// </summary>
    public class RichClientFrontend : ISetup<IConfigurableBus>
    {
        public void Accept(IConfigurableBus setup)
        {
            setup.ConfigurePublishing(p => p.DefaultPublishPipeline(new ParallelBlockingPublisher()));
            setup.AddResolver(new CompositeSubscription());
            setup.ConfigureSubscribing(cs => cs.ApplyOnNewSubscription(new ShapeToDispose()));
            setup.AddService(TaskScheduler.FromCurrentSynchronizationContext());
        }
    }

    /// <summary>
    /// Setup the Bus with this on the UI thread (Important!).
    /// A parallel publisher is used that publishes messages in parallel. With this setup <see cref="IBus.Publish"/> does NOT block.
    /// Exceptions will become available once all subscriptions are done processing the message as <see cref="ExceptionOccurred"/> message.
    /// This setup allows you to call <see cref="SubscriptionCustomizer{M}.DispatchOnUiThread"/> when doing a subscription.
    /// </summary>
    public class AsyncRichClientFrontend : AsyncConfiguration
    {
        public override void Accept(IConfigurableBus setup)
        {
            base.Accept(setup);
            setup.AddService(TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}