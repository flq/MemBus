using System;
using MemBus.Support;

namespace MemBus.Subscribing
{
    public class DisposableSubscription : IDisposableSubscription, IDisposable
    {
        private ISubscription action;

        public DisposableSubscription(ISubscription action)
        {
            this.action = action;
        }

        public void Push(object message)
        {
            action.Push(message);
        }

        public IDisposable GetDisposer()
        {
            return this;
        }

        public event EventHandler Disposed;

        private void raiseDispose()
        {
            action = null;
            Disposed.Raise(this);
        }

        public Type Handles
        {
            get { return action.Handles; }
        }

        void IDisposable.Dispose()
        {
            raiseDispose();
        }
    }
}