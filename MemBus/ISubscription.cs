using System;

namespace MemBus
{
    public interface ISubscription
    {
        void Push(object message);
        IDisposable GetDisposer();
        event EventHandler Disposed;
        Type Handles { get; }
    }
}