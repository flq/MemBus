using MemBus.Publishing;
using MemBus.Setup;
using MemBus.Subscribing;

namespace MemBus.Configurators
{
    /// <summary>
    /// Sets up a SequentialPublisher, and a table based resolver.
    /// The message push process is interrupted when an exception is raised by a subscription.
    /// </summary>
    public class Conservative : ISetup<IConfigurableBus>
    {
        public void Accept(IConfigurableBus setup)
        {
            setup.ConfigurePublishing(p => p.DefaultPublishPipeline(new SequentialPublisher()));
            setup.AddResolver(new TableBasedResolver());
            setup.ConfigureSubscribing(cs => cs.ApplyOnNewSubscription(new ShapeToDispose()));
        }
    }
}