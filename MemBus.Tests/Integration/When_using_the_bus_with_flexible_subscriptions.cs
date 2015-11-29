using System;
using MemBus.Configurators;
using MemBus.Subscribing;
using MemBus.Tests.Help;
using System.Linq;

using NUnit.Framework;

namespace MemBus.Tests.Integration
{

    public class When_Using_The_Bus_With_Flexible_Subscriptions
    {
        private IBus _bus;
        private SomeHandler _handler;
        private IDisposable _handlerDisposer;

        public When_Using_The_Bus_With_Flexible_Subscriptions()
        {
            _bus = ConstructBusForHandle();
            _handler = new SomeHandler();
            _handlerDisposer = _bus.Subscribe(_handler);
        }

        [Test]
        public void Without_flexibility_setup_subscribing_instance_throws()
        {
            var b = BusSetup.StartWith<Conservative>().Construct();
            new Action(() => b.Subscribe(new SomeHandler())).Throws<InvalidOperationException>();
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
            _handler.MsgBCalls.ShouldBeEqualTo(1);
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
        public void returning_null_is_tolerated()
        {
            var h = new HandlerReturningNull();
            using (_bus.Subscribe(h))
            {
                _bus.Publish("Causing a null being returned from a route");
                h.MsgCall.ShouldBeEqualTo(1);
            }
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

            msgB.ShouldBeEqualTo(1);
            msgC.ShouldBeEqualTo(1);
        }

        [Test]
        public void void_method_subscription_correct_returns_known_instance()
        {
            var h = new SomeHandler();
            var sb = new MethodScanner("Handle").MakeBuilder();
            var subs = sb.BuildSubscriptions(h).OfType<IKnowsSubscribedInstance>().ToList();
            subs.ShouldHaveCount(1);
            subs.All(s => s.Instance.Equals(h)).ShouldBeTrue("Not all known instances are the correct one");
        }

        [Test]
        public void publishing_method_subscription_correct_returns_known_instance()
        {
            var h = new SomeHandler();
            var sb = new MethodScanner("Route").MakeBuilder();
            var subs = sb.BuildSubscriptions(h).OfType<IKnowsSubscribedInstance>().ToList();
            subs.ShouldHaveCount(1);
            subs.All(s => s.Instance.Equals(h)).ShouldBeTrue("Not all known instances are the correct one");
        }

        private static IBus ConstructBusForHandle()
        {
            return BusSetup.StartWith<Conservative>()
                .Apply<FlexibleSubscribeAdapter>(c => c.RegisterMethods("Handle").RegisterMethods("Route")).Construct();
        }
    }
}