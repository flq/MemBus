using System;
using MemBus.Support;

namespace MemBus.Subscribing
{
    public class DisposableSubscription : IDisposableSubscription, IDenyShaper, IDisposable, IKnowsSubscribedInstance
    {
        private ISubscription action;

        public DisposableSubscription(ISubscription action)
        {
            this.action = action;
        }

        public void Push(object message)
        {
            if (!IsDisposed)
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
            IsDisposed = true;
            action = null;
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

        public object Instance
        {
            get { return (action as IKnowsSubscribedInstance).IfNotNull(ks => ks.Instance); }
        }
    }
}