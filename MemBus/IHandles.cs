namespace MemBus
{
    /// <summary>
    /// Type-safe version of a <see cref="ISubscription"/>
    /// </summary>
    public interface IHandles<in T> : ISubscription
    {
        /// <summary>
        /// Accept a message of type T.
        /// </summary>
        void Push(T message);
    }
}