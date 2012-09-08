using MemBus.Subscribing;
using MemBus.Tests.Help;
using MemBus.Tests.Frame;

#if WINRT
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
using TestFixtureSetup = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestInitializeAttribute;
#else
using NUnit.Framework;
#endif

namespace MemBus.Tests.Subscribing
{
    [TestFixture]
    public class Using_subscription_shapes
    {
        [Test]
        public void Correct_Sequence_Of_Matroschka()
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