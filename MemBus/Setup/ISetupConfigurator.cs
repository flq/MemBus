namespace MemBus
{
    public interface ISetupConfigurator<T>
    {
        void Accept(T setup);
    }
}