using System;
using MemBus.Subscribing;
using MemBus.Support;

namespace MemBus.Tests.Help
{
    public class TestShaper : ISubscriptionShaper, IRequireServices
    {
        private readonly string name;
        private readonly Action actionCalledOnPublish;

        public TestShaper(string name, Action actionCalledOnPublish)
        {
            this.name = name;
            this.actionCalledOnPublish = actionCalledOnPublish;
        }

        public TestShaper(string name)
            : this(name, null)
        {
            this.name = name;
        }

        public IServices Services { get; private set; }

        public void AddServices(IServices svc) {
            Services = svc;
        }

        public ISubscription EnhanceSubscription(ISubscription subscription)
        {
            return new NamedSubscription(name, actionCalledOnPublish, subscription);
        }
    }
}