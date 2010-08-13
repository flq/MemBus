using System;
using MemBus.Subscribing;

namespace MemBus.Tests.Help
{
    internal class TestShaper : ISubscriptionShaper
    {
        private readonly string name;
        private readonly Action action;

        public TestShaper(string name, Action action)
        {
            this.name = name;
            this.action = action;
        }

        public TestShaper(string name)
            : this(name, null)
        {
            this.name = name;
        }

        public ISubscription EnhanceSubscription(ISubscription subscription)
        {
            return new NamedSubscription(name, action, subscription);
        }
    }
}