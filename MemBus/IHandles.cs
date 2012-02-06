using System;

namespace MemBus
{
    /// <summary>
    /// Type-safe version of a <see cref="ISubscription"/>
    /// </summary>
    [Obsolete("In a next release MemBus will not support this interface. It was used in the context of resolving handlers from Ioc. You are now able to define your own Handler interface")]
    public interface IHandles<in T> : ISubscription
    {
        /// <summary>
        /// Accept a message of type T.
        /// </summary>
        void Push(T message);
    }
}