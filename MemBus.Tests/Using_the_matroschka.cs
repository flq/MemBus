using MemBus.Subscribing;
using MemBus.Tests.Help;
using NUnit.Framework;
using MemBus.Tests.Frame;

namespace MemBus.Tests
{
    [TestFixture]
    public class Using_the_matroschka
    {
        [Test]
        public void Correct_Sequence_Of_Matroschka()
        {
            var m = new SubscriptionMatroschkaFactory {new TestShaper("A"), new TestShaper("B")};
            var s = (NamedSubscription)m.EnhanceSubscription(new NamedSubscription("First", null));
            s.Name.ShouldBeEqualTo("B");
            ((NamedSubscription)s.Inner).Name.ShouldBeEqualTo("A");
        }

        [Test]
        public void Next_to_inner_produces_correct_sequence()
        {
            var m = new SubscriptionMatroschkaFactory { new TestShaper("A") };
            m.AddNextToInner(new TestShaper("B"));
            var s = (NamedSubscription)m.EnhanceSubscription(new NamedSubscription("First", null));
            s.Name.ShouldBeEqualTo("A");
            ((NamedSubscription)s.Inner).Name.ShouldBeEqualTo("B");
        }
    }
}