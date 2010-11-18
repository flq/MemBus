using MemBus.Tests.Help;
using Moq;
using NUnit.Framework;
using System.Linq;
using MemBus.Tests.Frame;

namespace MemBus.Tests
{
    [TestFixture]
    public class When_using_caching_resolver
    {
        

        [Test]
        public void Caching_resolver_accepts_and_returns_subscriptions()
        {
            var sub = Helpers.MockSubscriptionThatHandles<MessageA>();
            var r = new CachingResolver();
            r.Add(sub.Object);
            var subs = r.GetSubscriptionsFor(new MessageA()).ToList();
            subs.ShouldHaveCount(1);
        }

        [Test]
        public void Sequence_of_addition_and_disposal_works_as_shown()
        {
            var sub1 = new MockSubscription<MessageA>();
            var sub2 = new MockSubscription<MessageA>();
            var sub3 = new MockSubscription<MessageB>();
            
            var r = new CachingResolver();
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
    }

}