using MemBus.Configurators;
using MemBus.Subscribing;
using MemBus.Tests.Help;
using NUnit.Framework;

namespace MemBus.Tests.Subscribing
{

    [TestFixture]
    public class Using_predicate_based_subscribing
    {
        private IBus _bus;
        private readonly SomeHandler _handler = new SomeHandler();

        [TestFixtureSetUp]
        public void Given()
        {
            _bus = BusSetup
                .StartWith<Conservative>()
                .Apply<FlexibleSubscribeAdapter>(adp => adp.ByMethodName("Handle").PickUpMethods(mi => true))
                .Construct();
            _bus.Subscribe(_handler);
            _bus.Publish(new MessageA());
            _bus.Publish(new MessageB());
            _bus.Publish(new MessageC());
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