using System;

namespace MemBus
{
    public interface ISubscription
    {
        void Push(object message);
        IDisposable GetDisposer();
        Type Handles { get; }
    }
}