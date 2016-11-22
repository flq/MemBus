using MemBus;
using System;

namespace Membus.Tests.Help
{
    public class SubscriptionThatFakesHandles<T> : ISubscription
    {
        public int PushCalls;

        public bool Handles(Type messageType)
        {
            return messageType == typeof(T);
        }

        void ISubscription.Push(object message)
        {
            PushCalls++;
        }
    }
}
