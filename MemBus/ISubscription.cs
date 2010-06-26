using System;

namespace MemBus
{
    public interface ISubscription
    {
        void Push(object message);
        Type Handles { get; }
    }
}