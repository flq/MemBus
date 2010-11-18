using System;
using System.Threading;
using MemBus.Subscribing;
using MemBus.Support;

namespace MemBus.Tests.Help
{
    public class MockSubscription<T> : IDisposableSubscription, IDisposable
    {
        private readonly ManualResetEvent evtBlock;
        private readonly ManualResetEvent evtSignal;
        public int Received;

        public MockSubscription()
        {
        }

        public MockSubscription(ManualResetEvent evtBlock = null, ManualResetEvent evtSignal = null)
        {
            this.evtBlock = evtBlock;
            this.evtSignal = evtSignal;
        }

        public void Push(object message)
        {
            if (evtBlock != null)
                evtBlock.WaitOne();
            Received++;
            if (evtSignal != null)
              evtSignal.Set();
        }

        public bool Handles(Type messageType)
        {
            return typeof(T).IsAssignableFrom(messageType);
        }

        public IDisposable GetDisposer()
        {
            return this;
        }

        public bool IsDisposed
        {
            get; private set;
        }

        public event EventHandler Disposed;

        public void Dispose()
        {
            IsDisposed = true;
            Disposed.Raise(this);
        }
    }

    public class DenyingSubscription : ISubscription, IDenyShaper
    {
        public void Push(object message)
        {
            
        }

        public bool Handles(Type messageType)
        {
            return messageType.Equals(typeof (MessageA));
        }


        public bool Deny
        {
            get { return true; }
        }
    }
}