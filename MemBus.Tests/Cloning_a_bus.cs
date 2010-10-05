using System;
using System.Text;
using MemBus.Configurators;
using MemBus.Tests.Help;
using NUnit.Framework;
using MemBus.Tests.Frame;

namespace MemBus.Tests
{
    [TestFixture]
    public class Cloning_a_bus
    {
        [Test]
        public void Call_clone_to_non_cloneable_not_allowed()
        {
            var b = BusSetup.StartWith<Conservative>().Construct();
            Assert.Throws<InvalidOperationException>(() => { var b2 = b.Clone(); });
        }

        [Test]
        public void Calling_cloneable_on_setup_creates_new_instance()
        {
            var b = BusSetup.StartWith<Conservative>().ConstructCloneable();
            var b2 = b.Clone();
            ReferenceEquals(b, b2).ShouldBeFalse();
        }

        [Test]
        public void For_publish_cloned_bus_acts_as_original()
        {
            var sb = new StringBuilder();
            var bParent = BusSetup.StartWith<Conservative>(new BusSetupWithTestShapers(sb)).ConstructCloneable();
            var bus = bParent.Clone();

            using (bus.Subscribe<MessageB>(msg => { }))
                bus.Publish(new MessageB());

            sb.ToString().ShouldBeEqualTo("AB"); 
        }

        [Test]
        public void A_cloned_bus_cannot_be_cloned_again()
        {
            var b = BusSetup.StartWith<Conservative>().ConstructCloneable();
            var b2 = b.Clone();
            Assert.Throws<InvalidOperationException>(() => { var b3 = b2.Clone(); });
        }

    }
}