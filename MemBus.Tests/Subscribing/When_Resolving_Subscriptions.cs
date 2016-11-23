using MemBus.Tests.Help;
using System.Linq;
using Xunit;


namespace MemBus.Tests.Subscribing
{
    public class When_Resolving_Subscriptions
    {
        private CompositeResolver _resolver;

        public When_Resolving_Subscriptions()
        {
            _resolver = new CompositeResolver(new SimpleResolver {new MockSubscription<MessageA>()},
                                             new SimpleResolver { new MockSubscription<MessageA>(), new MockSubscription<MessageB>()});
        }

        [Fact]
        public void returns_single_subscription_for_msg_b()
        {
            var subs = _resolver.GetSubscriptionsFor(new MessageB());
            subs.ShouldHaveCount(1);
            subs.Single().Handles(typeof(MessageB)).ShouldBeTrue();
        }

        [Fact]
        public void returns_none_for_msg_c()
        {
            var subs = _resolver.GetSubscriptionsFor(new MessageC());
            subs.ShouldHaveCount(0);
        }

        [Fact]
        public void returns_both_msg_a_subscriptions()
        {
            var subs = _resolver.GetSubscriptionsFor(new MessageA());
            subs.ShouldHaveCount(2);
        }

    }

}