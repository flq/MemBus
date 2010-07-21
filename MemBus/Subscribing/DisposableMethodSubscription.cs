using System;
using MemBus.Support;

namespace MemBus.Subscribing
{
    public class DisposableMethodSubscription<T> : ISubscription
    {
        private Action<T> action;

        public DisposableMethodSubscription(Action<T> action)
        {
            this.action = action;
        }

        public void Push(object message)
        {
            action((T) message);
        }

        public IDisposable GetDisposer()
        {
            return new PrivateDisposer(this);
        }

        public event EventHandler Disposed;

        private void raiseDispose()
        {
            action = null;
            Disposed.Raise(this);
        }

        public Type Handles
        {
            get { return typeof(T); }
        }

        private class PrivateDisposer : IDisposable
        {
            private readonly DisposableMethodSubscription<T> disposableMethodSubscription;

            public PrivateDisposer(DisposableMethodSubscription<T> disposableMethodSubscription)
            {
                this.disposableMethodSubscription = disposableMethodSubscription;
            }

            public void Dispose()
            {
                disposableMethodSubscription.raiseDispose();
            }
        }
    }
}