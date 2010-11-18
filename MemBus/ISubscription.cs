using System;

namespace MemBus
{
    public interface ISubscription
    {
        void Push(object message);
        bool Handles(Type messageType);
    }

    public interface IDisposableSubscription : ISubscription
    {
        IDisposable GetDisposer();
        bool IsDisposed { get; }
        event EventHandler Disposed;
    }
}