using System;

namespace MemBus
{
    public interface ISubscription
    {
        void Push(object message);
        Type Handles { get; }
    }

    public interface IDisposableSubscription : ISubscription
    {
        IDisposable GetDisposer();
        event EventHandler Disposed;
    }
}