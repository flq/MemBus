using System;
using MemBus.Subscribing;

namespace MemBus
{
    /// <summary>
    /// Sets up a SequentialPublisher, and a table based resolver.
    /// The message push process is interrupted when an exception is raised by a subscription.
    /// Subscriptions need to be managed yourself with the IDisposable return value.
    /// </summary>
    public class Conservative : IBusSetupConfigurator
    {
        public void Accept(IConfigurableBus setup)
        {
            setup.InsertPublishPipelineMember(new SequentialPublisher());
            setup.InsertResolver(new TableBasedResolver());
            setup.AddService(new SubscriptionMatroschkaFactory { new ShapeToDispose() });
        }
    }
}