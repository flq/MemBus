using System.Collections.Generic;
using MemBus.Subscribing;
using MemBus.Tests.Help;
using Xunit;

namespace MemBus.Tests.Subscribing
{
    public class FSA_Using_Interface_Based_Subscribing : FlexibleSubscribingIntegrationContext
    {
        private readonly SomeHandler _handler = new SomeHandler();

        protected override void ConfigureAdapter(FlexibleSubscribeAdapter adp)
        {
            adp.ByInterface(typeof(IWeirdHandler<>));
        }

        protected override IEnumerable<object> GetEndpoints()
        {
            yield return _handler;
        }

        [Fact]
        public void Msg_A_was_received_as_produced_by_observable()
        {
            _handler.MsgACalls.ShouldBeEqualTo(1);
        }
    }

}