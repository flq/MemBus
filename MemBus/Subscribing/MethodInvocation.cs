using System;

namespace MemBus.Subscribing
{
    /// <summary>
    /// Helper to build a <see cref="MethodInvocation{T}"/>. See <see cref="FlexibleSubscribeAdapter"/> why.
    /// </summary>
    public class MethodInvocation
    {
        public MethodInvocation<T> Build<T>(Action<T> action)
        {
            return new MethodInvocation<T>(action);
        }
    }

    public class MethodInvocation<T> : ISubscription
    {
        private readonly Action<T> action;

        public MethodInvocation(Delegate action) : this((Action<T>)action)
        {
        }

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