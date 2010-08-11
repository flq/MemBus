using MemBus.Subscribing;

namespace MemBus.Tests.Help
{
    internal class TestShaper : ISubscriptionShaper
    {
        private readonly string name;

        public TestShaper(string name)
        {
            this.name = name;
        }

        public ISubscription EnhanceSubscription(ISubscription subscription)
        {
            return new NamedSubscription(name, subscription);
        }
    }
}