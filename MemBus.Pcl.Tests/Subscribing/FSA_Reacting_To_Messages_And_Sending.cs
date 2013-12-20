using System.Collections.Generic;
using System.Linq;
using MemBus.Subscribing;
using MemBus.Tests.Help;
using NUnit.Framework;

namespace MemBus.Tests.Subscribing
{
    [TestFixture]
    public class FSA_Reacting_To_Messages_And_Sending : FlexibleSubscribingIntegrationContext
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

            public void AssertContainsMessageOfType<T>(int count = 1)
            {
                Messages.OfType<T>().ShouldHaveCount(count);
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

        [TearDown]
        public void Teardown()
        {
            Clear();
            _tester.Clear();
        }

        [Test]
        public void string_msg_received()
        {
            Bus.Publish("Hello");

            _tester.AssertContainsMessageOfType<string>();
        }

        [Test]
        public void msg_a_reception_triggers_sending_msg_b()
        {
            Bus.Publish(new MessageA());

            _tester.AssertContainsMessageOfType<MessageA>(2);
            Messages.OfType<MessageB>().Count().ShouldBeEqualTo(1);
        }

        [Test]
        public void msg_a_reception_triggers_sending_c_messages()
        {
            Bus.Publish(new MessageA());
            Messages.OfType<MessageC>().Count().ShouldBeEqualTo(2);
        }
    }
}