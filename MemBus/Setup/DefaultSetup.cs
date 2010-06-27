namespace MemBus
{
    public class DefaultSetup : IBusSetupConfigurator
    {
        public void Accept(IConfigurableBus setup)
        {
            setup.InsertPublishPipeline(new SequentialPublisher());
            setup.InsertResolver(new TableBasedResolver());
        }
    }
}