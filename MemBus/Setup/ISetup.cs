namespace MemBus.Setup
{
    public interface ISetup<T>
    {
        void Accept(T setup);
    }
}