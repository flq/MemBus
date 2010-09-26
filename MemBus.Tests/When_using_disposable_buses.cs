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
       
    }
}