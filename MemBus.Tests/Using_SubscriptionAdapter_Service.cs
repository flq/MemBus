using System;
using System.Linq;
using MemBus.Setup;
using MemBus.Subscribing;
using MemBus.Tests.Help;
using Moq;
using NUnit.Framework;
using MemBus.Tests.Frame;
using MemBus.Support;

namespace MemBus.Tests
{
    [TestFixture]
    public class Using_SubscriptionAdapter_Service
    {
        [Test]
        public void Unconfigured_setup_will_throw_invalid_op()
        {
            var setup = new FlexibleSubscribeAdapter();
            Assert.Throws<InvalidOperationException>(() => ((ISetup<IConfigurableBus>)setup).Accept(null));
        }

        [Test]
        public void When_having_some_configuration_adapter_adds_itself_as_service()
        {
            var setup = new FlexibleSubscribeAdapter();
            setup.ByMethodName("Handle");

            var bus = new Mock<IConfigurableBus>();
            ((ISetup<IConfigurableBus>)setup).Accept(bus.Object);

            bus.Verify(c=>c.AddService<IAdapterServices>(setup));
        }

        [Test]
        public void Integrative_test_of_finding_all_handlers_in_complex_scenario()
        {
            var setup = new FlexibleSubscribeAdapter();
            setup
                .ByInterface(typeof(IClassicIHandleStuffI<>))
                .ByMethodName("Handle")
                .ByMethodName("Schmandle");
            
            var handler = new SomeCrazyHandler();
            var simpleResolver = new SimpleResolver();
            ((IAdapterServices) setup).WireUpSubscriber(simpleResolver, handler);

            var subs = simpleResolver.ToList();

            subs.ShouldHaveCount(5);

            subs.Where(s=>s.Handles(typeof(MessageASpecialization))).Each(s=>s.Push(new MessageASpecialization()));
            handler.MessageACount.ShouldBeEqualTo(1);
            handler.MsgSpecialACount.ShouldBeEqualTo(1);

            subs.Where(s => s.Handles(typeof(MessageB))).Each(s => s.Push(new MessageB()));
            handler.MessageBCount.ShouldBeEqualTo(2); //There are 2 handle methods for MsgB :)

            subs.Where(s => s.Handles(typeof(MessageC))).Each(s => s.Push(new MessageC()));
            handler.MessageCCount.ShouldBeEqualTo(1);
        }

        [Test]
        public void Subscriptions_are_built_for_object_method_based()
        {
            var builder = new VoidMethodBasedBuilder("Handle");
            var subs = builder.BuildSubscriptions(new SomeHandler());
            subs.ShouldNotBeNull();
            subs.ShouldHaveCount(1);
        }

        [Test]
        public void Subscriptions_for_object_method_based_work_correctly()
        {
            var builder = new VoidMethodBasedBuilder("Handle");
            var handler = new SomeHandler();
            var subs = builder.BuildSubscriptions(handler);
            var subscription = subs.First();
            subscription.Handles(typeof(MessageA)).ShouldBeTrue();
            subscription.Push(new MessageA());
            handler.MsgACalls.ShouldBeEqualTo(1);
        }

        [TestCase(typeof(IInvalidHandlerInterfaceBecauseNoParameter))]
        [TestCase(typeof(IInvalidHandlerInterfaceBecauseTwoMethodsOfrequestedPattern))]
        [TestCase(typeof(IInvalidHandlerInterfaceBecauseReturnType))]
        [TestCase(typeof(IInvalidHandlerInterfaceBecauseTwoParams))]
        public void These_handler_interfaces_are_invalid_for_usage(Type interfaceType)
        {
            Assert.Throws<InvalidOperationException>(() => { new InterfaceBasedBuilder(interfaceType); });
        }

        [Test]
        public void Non_generic_interface_is_properly_handled()
        {
            var builder = new InterfaceBasedBuilder(typeof(ItfNonGenericForHandles));
            var targetToAdapt = new AHandlerThroughSimpleInterface();
            var subs = builder.BuildSubscriptions(targetToAdapt);
            subs.ShouldHaveCount(1);
            var s = subs.First();
            s.Handles(typeof(MessageA)).ShouldBeTrue();
            s.Push(new MessageA());
            targetToAdapt.MsgCount.ShouldBeEqualTo(1);
        }

        [Test]
        public void Two_subscriptions_expected_from_aquainting_crazy_handler()
        {
            var builder = new InterfaceBasedBuilder(typeof (IClassicIHandleStuffI<>));
            var subs = builder.BuildSubscriptions(new SomeCrazyHandler());
            subs.ShouldHaveCount(2);
        }

        [Test]
        public void explicit_implementation_of_interfaces_is_supported()
        {
            var builder = new InterfaceBasedBuilder(typeof(IClassicIHandleStuffI<>));
            var subs = builder.BuildSubscriptions(new HandlerWithExplicitImpl());
            subs.ShouldHaveCount(1);
        }
    }
}