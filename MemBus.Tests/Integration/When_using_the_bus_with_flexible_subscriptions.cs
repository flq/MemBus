using System;
using MemBus.Configurators;
using MemBus.Subscribing;
using MemBus.Tests.Help;
using NUnit.Framework;
using MemBus.Tests.Frame;
using System.Linq;

namespace MemBus.Tests.Integration
{
    [TestFixture]
    public class When_using_the_bus_with_flexible_subscriptions
    {
        private IBus _bus;
        private SomeHandler _handler;
        private IDisposable _handlerDisposer;

        [SetUp]
        public void Given()
        {
            _bus = ConstructBusForHandle();
            _handler = new SomeHandler();
            _handlerDisposer = _bus.Subscribe(_handler);
        }

        [Test]
        public void Without_flexibility_setup_subscribing_instance_throws()
        {
            var b = BusSetup.StartWith<Conservative>().Construct();
            Assert.Throws<InvalidOperationException>(() => b.Subscribe(new SomeHandler()));
        }

        [Test]
        public void Basic_publishing_reaches_the_flexible_subscription()
        {
            _bus.Publish(new MessageA());
            _handler.MsgACalls.ShouldBeEqualTo(1);
        }

        [Test]
        public void Disposal_also_works_on_flexible_subscriptions()
        {            
            _bus.Publish(new MessageA());
            _handlerDisposer.Dispose();
            _bus.Publish(new MessageA());
            _handler.MsgACalls.ShouldBeEqualTo(1);
        }

        [Test]
        public void Handler_may_accept_its_own_dispose_token()
        {
            _bus.Publish(new MessageA());
            _handler.MsgACalls.ShouldBeEqualTo(1);
            _handler.InvokeDisposeToken();
            _bus.Publish(new MessageA());
            _handler.MsgACalls.ShouldBeEqualTo(1);

        }

        [Test]
        public void Related_to_caching_resolver_failed_publish()
        {
            _bus.Publish(new MessageA());
            _handler.MsgACalls.ShouldBeEqualTo(1);
            _handler.InvokeDisposeToken();
            
            _handler = new SomeHandler();
            _handler.MsgACalls.ShouldBeEqualTo(0);
            _bus.Subscribe(_handler);

            _bus.Publish(new MessageB());
            _bus.Publish(new MessageA());

            _handler.MsgACalls.ShouldBeEqualTo(1);
        }

        [Test]
        public void method_with_return_type_can_be_treated_as_subscription()
        {
            _bus.Publish(new MessageB());
            _handler.MsgBCalls.ShouldBeEqualTo(1, "MsgB was not received");
        }
        
        [Test]
        public void return_type_of_a_subscription_is_treated_as_message()
        {
            MessageC msgC = null;
            _bus.Subscribe((MessageC msg) => msgC = msg);
            _bus.Publish(new MessageB());

            msgC.ShouldNotBeNull();
            msgC.ShouldBeEqualTo(_handler.MsgC);
        }

        [Test]
        public void return_type_enumerable_publishes_every_item_as_message()
        {
            var msgB = 0;
            var msgC = 0;
            _bus.Subscribe(new EnumeratingHandler());
            _bus.Subscribe((MessageB m) => msgB++);
            _bus.Subscribe((MessageB m) => msgC++);
            _bus.Publish(new MessageA());

            msgB.ShouldBeEqualTo(1, "No MessageB received");
            msgC.ShouldBeEqualTo(1, "No MessageC received");
        }

        [Test]
        public void void_method_subscription_correct_returns_known_instance()
        {
            var h = new SomeHandler();
            var sb = new VoidMethodBasedBuilder("Handle");
            var subs = sb.BuildSubscriptions(h).OfType<IKnowsSubscribedInstance>().ToList();
            subs.ShouldHaveCount(1);
            subs.All(s => s.Instance.Equals(h)).ShouldBeTrue("Not all known instances are the correct one");
        }

        [Test]
        public void publishing_method_subscription_correct_returns_known_instance()
        {
            var h = new SomeHandler();
            var sb = new ReturningMethodBasedBuilder("Route");
            var subs = sb.BuildSubscriptions(h).OfType<IKnowsSubscribedInstance>().ToList();
            subs.ShouldHaveCount(1);
            subs.All(s => s.Instance.Equals(h)).ShouldBeTrue("Not all known instances are the correct one");
        }

        private static IBus ConstructBusForHandle()
        {
            return BusSetup.StartWith<Conservative>()
                .Apply<FlexibleSubscribeAdapter>(c => c.ByMethodName("Handle").PublishMethods("Route")).Construct();
        }
    }
}