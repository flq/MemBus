using System;
using MemBus.Configurators;
using MemBus.Subscribing;
using MemBus.Tests.Help;
using NUnit.Framework;
using MemBus.Tests.Frame;

namespace MemBus.Tests
{
    [TestFixture]
    public class When_using_the_bus_with_flexible_subscriptions
    {

        [Test]
        public void Without_flexibility_setup_subscribing_instance_throws()
        {
            var b = BusSetup.StartWith<Conservative>().Construct();
            Assert.Throws<InvalidOperationException>(() => b.Subscribe(new SomeHandler()));
        }

        [Test]
        public void Basic_publishing_reaches_the_flexible_subscription()
        {
            var b = BusSetup.StartWith<Conservative>()
                .Apply<FlexibleSubscribeAdapter>(c => c.ByMethodName("Handle")).Construct();
            var handler = new SomeHandler();
            b.Subscribe(handler);
            b.Publish(new MessageA());
            handler.MsgACalls.ShouldBeEqualTo(1);
        }

        [Test]
        public void Disposal_Also_works_on_flexible_subscriptions()
        {
            var b = BusSetup.StartWith<Conservative>()
                .Apply<FlexibleSubscribeAdapter>(c => c.ByMethodName("Handle")).Construct();
            var handler = new SomeHandler();
            var d = b.Subscribe(handler);
            b.Publish(new MessageA());
            d.Dispose();
            b.Publish(new MessageA());
            handler.MsgACalls.ShouldBeEqualTo(1);

        }
    }
}