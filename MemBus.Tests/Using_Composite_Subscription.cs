using System;
using MemBus.Tests.Help;
using NUnit.Framework;
using MemBus.Tests.Frame;
using System.Linq;

namespace MemBus.Tests
{
    [TestFixture]
    public class Using_Composite_Subscription
    {
        [Test]
        public void empty_composite_does_not_know_handles_yet()
        {
            var c = new CompositeSubscription();
            Assert.Throws<InvalidOperationException>(() => c.Handles.ShouldNotBeNull());
        }

        [Test]
        public void first_add_calibrates_and_disallows_other_subscriptions()
        {
            var c = new CompositeSubscription {new MockSubscription<MessageA>()};
            Assert.Throws<InvalidOperationException>(() => c.Add(new MockSubscription<MessageB>()));
        }

        [Test]
        public void composite_enumerator_works()
        {
            var c = new CompositeSubscription { new MockSubscription<MessageA>(), new MockSubscription<MessageA>() };
            c.Where(s=>s.Handles.Equals(typeof(MessageA))).ShouldHaveCount(2);
        }

        [Test]
        public void push_delegates_to_all_childs()
        {
            var c = new CompositeSubscription() {new MockSubscription<MessageA>(), new MockSubscription<MessageA>()};
            c.Push(new MessageA());
            c.OfType<MockSubscription<MessageA>>().All(m=>m.Received == 1).ShouldBeTrue();
        }
    }
}