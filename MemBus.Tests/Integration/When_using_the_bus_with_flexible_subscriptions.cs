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
            var b = ConstructBusForHandle();
            var handler = new SomeHandler();
            b.Subscribe(handler);
            b.Publish(new MessageA());
            handler.MsgACalls.ShouldBeEqualTo(1);
        }

        [Test]
        public void Disposal_also_works_on_flexible_subscriptions()
        {
            var b = ConstructBusForHandle();
            var handler = new SomeHandler();
            var d = b.Subscribe(handler);
            b.Publish(new MessageA());
            d.Dispose();
            b.Publish(new MessageA());
            handler.MsgACalls.ShouldBeEqualTo(1);
        }

        [Test]
        public void Handler_may_accept_its_own_dispose_token()
        {
            var b = ConstructBusForHandle();
            var handler = new SomeHandler();
            b.Subscribe(handler);
            b.Publish(new MessageA());
            handler.MsgACalls.ShouldBeEqualTo(1);
            handler.InvokeDisposeToken();
            b.Publish(new MessageA());
            handler.MsgACalls.ShouldBeEqualTo(1);

        }

        [Test]
        public void Related_to_caching_resolver_failed_publish()
        {
            var b = ConstructBusForHandle();
            var handler = new SomeHandler();
            b.Subscribe(handler);
            b.Publish(new MessageA());
            handler.MsgACalls.ShouldBeEqualTo(1);
            handler.InvokeDisposeToken();
            
            
            handler = new SomeHandler();
            handler.MsgACalls.ShouldBeEqualTo(0);
            b.Subscribe(handler);

            b.Publish(new MessageB());
            b.Publish(new MessageA());

            handler.MsgACalls.ShouldBeEqualTo(1);
        }

        [Test]
        public void method_with_return_type_can_be_treated_as_subscription()
        {
            var b = ConstructBusForHandle();
            var handler = new SomeHandler();
            b.Subscribe(handler);
            b.Publish(new MessageB());

            handler.MsgBCalls.ShouldBeEqualTo(1, "MsgB was not received");
        }
        
        [Test]
        public void return_type_of_a_subscription_is_treated_as_message()
        {
            MessageC msgC = null;
            var b = ConstructBusForHandle();
            var handler = new SomeHandler();
            b.Subscribe(handler);
            b.Subscribe((MessageC msg) => msgC = msg);
            b.Publish(new MessageB());

            msgC.ShouldNotBeNull();
            msgC.ShouldBeEqualTo(handler.MsgC);
        }

        private static IBus ConstructBusForHandle()
        {
            return BusSetup.StartWith<Conservative>()
                .Apply<FlexibleSubscribeAdapter>(c => c.ByMethodName("Handle").PublishMethods("Route")).Construct();
        }
    }
}