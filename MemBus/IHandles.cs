namespace MemBus
{
    public interface IHandles<in T> : ISubscription
    {
        void Push(T message);
    }
}