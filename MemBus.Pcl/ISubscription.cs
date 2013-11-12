using System;

namespace MemBus
{
    /// <summary>
    /// Basic ISubscription interface
    /// </summary>
    public interface ISubscription
    {
        /// <summary>
        /// Accept the message
        /// </summary>
        void Push(object message);

        /// <summary>
        /// State whether this type of message is handled
        /// </summary>
        bool Handles(Type messageType);
    }

    /// <summary>
    /// A disposable subscription
    /// </summary>
    public interface IDisposableSubscription : ISubscription
    {
        /// <summary>
        /// Get a handle to be able to dispose this subscription
        /// </summary>
        /// <returns></returns>
        IDisposable GetDisposer();

        /// <summary>
        /// State whether this subscription is disposed
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Raise event when you get disposed
        /// </summary>
        event EventHandler Disposed;
    }
}