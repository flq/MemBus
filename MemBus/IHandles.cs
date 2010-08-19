namespace MemBus
{
    /// <summary>
    /// Type-safe version of a <see cref="ISubscription"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHandles<in T> : ISubscription
    {
        void Push(T message);
    }
}