namespace MemBus.Setup
{
    public interface ISetupConfigurator<T>
    {
        void Accept(T setup);
    }
}