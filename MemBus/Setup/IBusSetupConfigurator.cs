namespace MemBus
{
    public interface IBusSetupConfigurator
    {
        void Accept(IConfigurableBus setup);
    }
}