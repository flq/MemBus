using System.Linq;
using MemBus.Subscribing;
using MemBus.Tests.Help;

using Xunit;

namespace MemBus.Tests.Publishing
{

    internal class Using_Publishing_Methods
    {
        private MessageEndpointsBuilder _builder;
        private FakeBus _bus;

        public Using_Publishing_Methods()
        {
            _bus = new FakeBus();
            _builder = new MethodScanner("Route").MakeBuilder();
            _builder.SetPublisher(_bus);
        }

        [Fact]
        public void Subscriptions_for_publishing_method_based_work_correctly()
        {
            var handler = new SomeHandler();
            var subscription = _builder.BuildSubscriptions(handler).First();
            subscription.Push(new MessageB());

            subscription.Handles(typeof(MessageB)).ShouldBeTrue();
            handler.MsgBCalls.ShouldBeEqualTo(1);
            _bus.VerifyMessageIsOfType<MessageC>();
        }

        [Fact]
        public void primitive_type_is_used_as_return_value()
        {
            var handler = new HandlerWithRouteReturningPrimitive();
            var subscription = _builder.BuildSubscriptions(handler).First();

            subscription.Push("Hello");

            subscription.Handles(typeof(string)).ShouldBeTrue();
            handler.MsgCall.ShouldBeEqualTo(1);
            _bus.VerifyMessageIsOfType<int>();
        }
    }
}
