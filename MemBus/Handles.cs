using System;

namespace MemBus
{
    /// <summary>
    /// Base class to write a handler that handles messages of the given param type
    /// </summary>
    public abstract class Handles<T> : IHandles<T>
    {
        public void Push(T message)
        {
            push(message);
        }

        void ISubscription.Push(object message)
        {
            Push((T)message);
        }

        protected abstract void push(T message);

        Type ISubscription.Handles
        {
            get { return typeof(T); }
        }
    }
}