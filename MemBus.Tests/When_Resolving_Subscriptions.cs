using MemBus.Tests.Help;
using Moq;
using NUnit.Framework;
using System.Linq;
using MemBus.Tests.Frame;

namespace MemBus.Tests
{
    [TestFixture]
    public class When_Resolving_Subscriptions
    {
        private CompositeResolver resolver;

        [TestFixtureSetUp]
        public void Given()
        {
            resolver = new CompositeResolver(new SimpleResolver {new MockSubscription<MessageA>()},
                                             new SimpleResolver { new MockSubscription<MessageA>(), new MockSubscription<MessageB>()});
        }

        [Test]
        public void returns_single_subscription_for_msg_b()
        {
            var subs = resolver.GetSubscriptionsFor(new MessageB());
            subs.ShouldHaveCount(1);
            subs.Single().Handles(typeof(MessageB)).ShouldBeTrue();
        }

        [Test]
        public void returns_none_for_msg_c()
        {
            var subs = resolver.GetSubscriptionsFor(new MessageC());
            subs.ShouldHaveCount(0);
        }

        [Test]
        public void returns_both_msg_a_subscriptions()
        {
            var subs = resolver.GetSubscriptionsFor(new MessageA());
            subs.ShouldHaveCount(2);
        }

    }

}