using MemBus.Subscribing;

namespace MemBus
{
    /// <summary>
    /// Sets up a SequentialPublisher, a table based resolver
    /// message will be pushed to other subscribers even though an exception is raised by a subscription.
    /// Subscriptions need to be managed yourself with the IDisposable return value.
    /// </summary>
    public class Default : IBusSetupConfigurator
    {
        public void Accept(IConfigurableBus setup)
        {
            setup.InsertPublishPipelineMember(new SequentialPublisher());
            setup.InsertResolver(new TableBasedResolver());
            setup.AddService<ISubscriptionShape>(new StandardShape());
        }
    }
}