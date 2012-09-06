using MemBus.Tests.Help;
using System.Linq;
using MemBus.Tests.Frame;

#if WINRT
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
using TestFixtureSetUp = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestInitializeAttribute;
#else
using NUnit.Framework;
#endif


namespace MemBus.Tests.Subscribing
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