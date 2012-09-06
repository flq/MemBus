using MemBus.Tests.Help;
using MemBus.Tests.Frame;
using System.Linq;

#if WINRT
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
#else
using NUnit.Framework;
#endif

namespace MemBus.Tests.Subscribing
{
    [TestFixture]
    public class Using_Composite_Subscription
    {
        [Test]
        public void empty_composite_would_handle_any_message()
        {
            var c = new CompositeSubscription();
            (c as ISubscription).Handles(typeof(MessageA)).ShouldBeTrue();
        }

        [Test]
        public void push_message_on_empty_composite_is_legal()
        {
            var c = new CompositeSubscription();
            (new System.Action(()=>(c as ISubscription).Push(new MessageA()))).DoesNotThrow();
        }

        [Test]
        public void adding_disparate_handlers_is_not_validated_by_composite()
        {
            var c = new CompositeSubscription {new MockSubscription<MessageA>(), new MockSubscription<MessageB>()};
            c.ShouldHaveCount(2);
        }

        [Test]
        public void composite_enumerator_works()
        {
            var c = new CompositeSubscription { new MockSubscription<MessageA>(), new MockSubscription<MessageA>() };
            c.ShouldHaveCount(2);
        }

        [Test]
        public void push_delegates_to_all_childs()
        {
            var c = new CompositeSubscription {new MockSubscription<MessageA>(), new MockSubscription<MessageA>()};
            c.Push(new MessageA());
            c.OfType<MockSubscription<MessageA>>().All(m=>m.Received == 1).ShouldBeTrue();
        }

        [Test]
        public void dispose_of_subscription_removes_it_from_composite()
        {
            var subscription = new MockSubscription<MessageA>();
            var c = new CompositeSubscription { subscription };
            c.ShouldHaveCount(1);
            subscription.Dispose();
            c.ShouldHaveCount(0);
        }
    }
}