using System;

namespace MemBus.Tests.Help
{
    public class MockSubscription<T> : ISubscription
    {
        public int Received;

        public void Push(object message)
        {
            Received++;
        }

        public Type Handles
        {
            get { return typeof(T); }
        }
    }
}