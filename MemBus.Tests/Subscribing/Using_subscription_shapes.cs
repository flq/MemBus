using MemBus.Subscribing;
using MemBus.Tests.Help;
using NUnit.Framework;

namespace MemBus.Tests.Subscribing
{
    [TestFixture]
    public class Using_subscription_shapes
    {
        [Test]
        public void Correct_sequence_of_matroschka()
        {
            var m = new SubscriptionShaperAggregate {new TestShaper("A"), new TestShaper("B")};
            var s = (NamedSubscription)m.EnhanceSubscription(new NamedSubscription("First", null));
            s.Name.ShouldBeEqualTo("B");
            ((NamedSubscription)s.Inner).Name.ShouldBeEqualTo("A");
        }

        [Test]
        public void Next_to_inner_produces_correct_sequence()
        {
            var m = new SubscriptionShaperAggregate { new TestShaper("A") };
            m.AddNextToInner(new TestShaper("B"));
            var s = (NamedSubscription)m.EnhanceSubscription(new NamedSubscription("First", null));
            s.Name.ShouldBeEqualTo("A");
            ((NamedSubscription)s.Inner).Name.ShouldBeEqualTo("B");
        }

        [Test]
        public void Denial_of_shape_correctly_propagates()
        {
            var s =
                new DisposableSubscription(
                    new FilteredSubscription<MessageA>(m => true,
                        new UiDispatchingSubscription(null,
                            new DenyingSubscription())));
            s.Deny.ShouldBeTrue();
        }
    }
}