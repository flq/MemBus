using System;
using MemBus.Support;

namespace MemBus.Subscribing
{
    public class DisposableSubscription : IDisposableSubscription, IDenyShaper, IDisposable
    {
        private ISubscription action;

        public DisposableSubscription() : this(new NullSubscription())
        {
            this.action = action;
        }

        public DisposableSubscription(ISubscription action)
        {
            this.action = action;
        }

        public void Push(object message)
        {
            action.Push(message);
        }

        public bool Handles(Type messageType)
        {
            return action.Handles(messageType);
        }

        public IDisposable GetDisposer()
        {
            return this;
        }

        public bool IsDisposed { get; private set; }

        public event EventHandler Disposed;

        private void raiseDispose()
        {
            action = null;
            IsDisposed = true;
            Disposed.Raise(this);
        }

        void IDisposable.Dispose()
        {
            raiseDispose();
        }

        public bool Deny
        {
            get { return action.CheckDenyOrAllIsGood(); }
        }

        private class NullSubscription : ISubscription
        {
            public void Push(object message)
            {
                
            }

            public bool Handles(Type messageType)
            {
                return false;
            }
        }
    }
}