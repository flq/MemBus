using System;

namespace MemBus.Subscribing
{
    public class MethodInvocation<T> : ISubscription
    {
        private readonly Action<T> action;

        public MethodInvocation(Action<T> action)
        {
            this.action = action;
        }

        public void Push(object message)
        {
            action((T)message);
        }

        public bool Handles(Type messageType)
        {
            return typeof (T).IsAssignableFrom(messageType);
        }
    }
}