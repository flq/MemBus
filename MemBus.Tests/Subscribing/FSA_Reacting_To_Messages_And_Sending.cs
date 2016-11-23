using MemBus.Subscribing;
using MemBus.Tests.Help;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MemBus.Tests.Subscribing
{
    public class FSA_Reacting_To_Messages_And_Sending : FlexibleSubscribingIntegrationContext, IDisposable
    {
        private readonly Tester _tester = new Tester();

        private class Tester : MessageReceiver
        {
            public void EOne(string message)
            {
                Messages.Add(message);
            }

            public MessageB ETwo(MessageA message)
            {
                Add(message);
                return new MessageB();
            }

            public IEnumerable<MessageC> EEnumerate(MessageA message)
            {
                Add(message);
                yield return new MessageC();
                yield return new MessageC();
            }

            public void Three(MessageB message)
            {
                Messages.Add(message);
            }
        }

        protected override IEnumerable<object> GetEndpoints()
        {
            yield return _tester;
        }

        protected override void ConfigureAdapter(FlexibleSubscribeAdapter adp)
        {
            adp.RegisterMethods(mi => mi.Name.StartsWith("E"));
        }

        [Fact]
        public void string_msg_received()
        {
            Bus.Publish("Hello");

            _tester.AssertContainsMessageOfType<string>();
        }

        [Fact]
        public void msg_a_reception_triggers_sending_msg_b()
        {
            Bus.Publish(new MessageA());

            _tester.AssertContainsMessageOfType<MessageA>(2);
            Messages.OfType<MessageB>().Count().ShouldBeEqualTo(1);
        }

        [Fact]
        public void msg_a_reception_triggers_sending_c_messages()
        {
            Bus.Publish(new MessageA());
            Messages.OfType<MessageC>().Count().ShouldBeEqualTo(2);
        }

        public void Dispose()
        {
            Clear();
            _tester.Clear();
        }
    }
}