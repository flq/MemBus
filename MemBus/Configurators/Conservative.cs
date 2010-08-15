using MemBus.Publishing;
using MemBus.Setup;
using MemBus.Subscribing;

namespace MemBus.Configurators
{
    /// <summary>
    /// Sets up a SequentialPublisher, and a table based resolver.
    /// The message push process is interrupted when an exception is raised by a subscription.
    /// Subscriptions need to be managed yourself with the IDisposable return value.
    /// </summary>
    public class Conservative : ISetupConfigurator<IConfigurableBus>
    {
        public void Accept(IConfigurableBus setup)
        {
            setup.ConfigurePublishing(p => p.DefaultPublishPipeline(new SequentialPublisher()));
            setup.InsertResolver(new TableBasedResolver());
            setup.ConfigureSubscribing(cs => cs.ShapeToApplyWhenIntroducingSubscription(new ShapeToDispose()));
        }
    }
}