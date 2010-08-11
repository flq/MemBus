using System;
using System.Threading;
using MemBus.Subscribing;
using MemBus.Support;

namespace MemBus.Tests.Help
{
    public class MockSubscription<T> : ISubscription, IDisposable
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

        public IDisposable GetDisposer()
        {
            return this;
        }

        public event EventHandler Disposed;

        public Type Handles
        {
            get { return typeof(T); }
        }

        public void Dispose()
        {
            Disposed.Raise(this);
        }
    }

    public class DenyingSubscription : ISubscription, IDenyShaper
    {
        public void Push(object message)
        {
            
        }

        public Type Handles
        {
            get { return typeof(MessageA); }
        }

        public bool Deny
        {
            get { return true; }
        }
    }
}