using System.Linq;
using MemBus.Subscribing;
using MemBus.Tests.Help;
using Xunit;

namespace MemBus.Tests
{
    
    public class ResolverTestContext
    {
        protected virtual ISubscriptionResolver GetResolver()
        {
            return new CompositeSubscription();
        }

        [Fact]
        public void Caching_resolver_accepts_and_returns_subscriptions()
        {
            var sub = Helpers.MockSubscriptionThatHandles<MessageA>();
            var r = (ISubscriptionResolver)new CompositeSubscription();
            r.Add(sub);
            var subs = r.GetSubscriptionsFor(new MessageA()).ToList();
            subs.ShouldHaveCount(1);
        }

        [Fact]
        public void Sequence_of_addition_and_disposal_works_as_shown()
        {
            var sub1 = new MockSubscription<MessageA>();
            var sub2 = new MockSubscription<MessageA>();
            var sub3 = new MockSubscription<MessageB>();

            var r = GetResolver();
            r.Add(sub1);
            r.GetSubscriptionsFor(new MessageA()).ShouldHaveCount(1);
            r.Add(sub2);
            r.GetSubscriptionsFor(new MessageA()).ShouldHaveCount(2);
            r.GetSubscriptionsFor(new MessageA()).ShouldHaveCount(2);
            r.Add(sub3);
            r.GetSubscriptionsFor(new MessageB()).ShouldHaveCount(1);
            sub1.Dispose();
            r.GetSubscriptionsFor(new MessageA()).ShouldHaveCount(1);
            sub3.Dispose();
            r.GetSubscriptionsFor(new MessageB()).ShouldHaveCount(0);

        }

        [Fact]
        public void correct_behavior_of_resolver_with_regard_to_subscribe_dispose_subscribe_publish_twice()
        {
            var sub1 = new MockSubscription<MessageA>();
            var r = GetResolver();
            
            r.Add(sub1);
            r.GetSubscriptionsFor(new MessageA()).ShouldHaveCount(1);
            sub1.Dispose();

            sub1 = new MockSubscription<MessageA>();
            r.Add(sub1);
            r.GetSubscriptionsFor(new MessageB()).ShouldHaveCount(0);
            r.GetSubscriptionsFor(new MessageA()).ShouldHaveCount(1);
        }

        [Fact]
        public void correct_behavior_subscribe_get_subscribe_new_then_get_old()
        {
            var sub1 = new MockSubscription<MessageA>();
            var sub2 = new MockSubscription<MessageB>();
            var r = GetResolver();

            r.Add(sub1);
            r.GetSubscriptionsFor(new MessageA()).ShouldHaveCount(1);
            r.Add(sub2);
            r.GetSubscriptionsFor(new MessageA()).ShouldHaveCount(1);
        }

        [Fact]
        public void correct_behavior_regarding_contravariance()
        {
            var sub = new MethodInvocation<Clong>(f => {});
            var r = GetResolver();
            r.Add(sub);
            r.GetSubscriptionsFor(new Clong()).ShouldHaveCount(1);
            r.GetSubscriptionsFor(new Clung()).ShouldHaveCount(1);
        }

        [Fact]
        public void correct_behavior_not_getting_message_twice()
        {
            var sub = new MethodInvocation<Clong>(f => { });
            var r = GetResolver();
            r.Add(sub);
            r.GetSubscriptionsFor(new Clong()).ShouldHaveCount(1);
            r.GetSubscriptionsFor(new Clong()).ShouldHaveCount(1);
        }
    }

    public class Clong {}
    public class Clung : Clong {}

}