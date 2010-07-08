namespace MemBus
{
    public class DefaultSetup : IBusSetupConfigurator
    {
        public void Accept(IConfigurableBus setup)
        {
            setup.InsertPublishPipelineMember(new SequentialPublisher());
            setup.InsertResolver(new TableBasedResolver());
        }
    }
}