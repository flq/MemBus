using System.Collections.Generic;
using MemBus.Subscribing;
using MemBus.Tests.Help;
using NUnit.Framework;

namespace MemBus.Tests.Subscribing
{

    [TestFixture]
    public class FSA_Using_Predicate_Based_Subscribing : FlexibleSubscribingIntegrationContext
    {
        private readonly SomeHandler _handler = new SomeHandler();

        protected override void ConfigureAdapter(FlexibleSubscribeAdapter adp)
        {
            adp.RegisterMethods("Handle").RegisterMethods(mi => true);
        }

        protected override IEnumerable<object> GetEndpoints()
        {
            yield return _handler;
        }

        protected override void AdditionalSetup()
        {
            Bus.Publish(new MessageA());
            Bus.Publish(new MessageB());
            Bus.Publish(new MessageC());
        }

        [Test]
        public void Handle_was_found()
        {
            _handler.MsgACalls.ShouldBeEqualTo(1);
        }

        [Test]
        public void Route_was_found()
        {
            _handler.MsgBCalls.ShouldBeEqualTo(1);
        }

        [Test]
        public void Any_method_from_the_route_and_explicit()
        {
            _handler.MsgCCalls.ShouldBeEqualTo(2);
        }
    }

}