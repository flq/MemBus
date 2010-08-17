using System;
using MemBus.Configurators;
using MemBus.Tests.Help;
using NUnit.Framework;
using MemBus.Tests.Frame;

namespace MemBus.Tests
{
    [TestFixture]
    public class When_using_disposable_buses
    {
        [Test]
        public void A_disposed_bus_throws()
        {
            var bus = BusSetup.StartWith<Conservative>().Construct();
            bus.Dispose();
            Assert.Throws<ObjectDisposedException>(() => bus.Publish(new MessageA()));
            Assert.Throws<ObjectDisposedException>(() => bus.Subscribe<MessageA>(msg=>{}));
            Assert.Throws<ObjectDisposedException>(() => bus.Observe<MessageA>());
        }

        [Test]
        public void Child_bus_is_spawned_and_disposed_without_affecting_parent()
        {
            int received = 0;
            var bus = BusSetup.StartWith<Conservative>().Construct();
            bus.Subscribe<MessageA>(msg => received++);
            var child = bus.SpawnChild();
            child.Dispose();
            bus.Publish(new MessageA());
            received.ShouldBeEqualTo(1);
        }

        [Test]
        public void Child_bus_asymetric_publish_is_given()
        {
            int received = 0;
            int receivedChild = 0;
            var bus = BusSetup.StartWith<Conservative>().Construct();
            bus.Subscribe<MessageA>(msg => received++);
            
            var child = bus.SpawnChild();
            child.Subscribe<MessageA>(msg => receivedChild++);

            bus.Publish(new MessageA());
            received.ShouldBeEqualTo(1);
            receivedChild.ShouldBeEqualTo(1);

            child.Publish(new MessageA());
            received.ShouldBeEqualTo(1);
            receivedChild.ShouldBeEqualTo(2);
        }

        [Test]
        public void Child_bus_asymetric_publish_can_be_softened_with_exception()
        {
            int receivedA = 0;
            int receivedB = 0;
            int receivedChildA = 0;
            int receivedChildB = 0;
            var bus = BusSetup.StartWith<Conservative>(b => b.AddBubblingForMessageType<MessageB>()).Construct();
            bus.Subscribe<MessageA>(msg => receivedA++);
            bus.Subscribe<MessageB>(msg => receivedB++);

            var child = bus.SpawnChild();
            child.Subscribe<MessageA>(msg => receivedChildA++);
            child.Subscribe<MessageB>(msg => receivedChildB++);

            child.Publish(new MessageA());
            child.Publish(new MessageB());
            receivedA.ShouldBeEqualTo(0);
            receivedB.ShouldBeEqualTo(1);
            receivedChildA.ShouldBeEqualTo(1);
            receivedChildB.ShouldBeEqualTo(1);
            
        }
    }
}