using System;
using System.Threading;

namespace MemBus.Tests.Help
{
    public class MockSubscription<T> : ISubscription
    {
        private readonly ManualResetEvent evt;
        public int Received;

        public MockSubscription()
        {
        }

        public MockSubscription(ManualResetEvent evt)
        {
            this.evt = evt;
        }

        public void Push(object message)
        {
            if (evt != null)
                evt.WaitOne();
            Received++;
        }

        public Type Handles
        {
            get { return typeof(T); }
        }
    }
}