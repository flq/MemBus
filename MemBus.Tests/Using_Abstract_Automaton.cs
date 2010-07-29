using MemBus.Support;
using MemBus.Tests.Help;
using Moq;
using NUnit.Framework;

namespace MemBus.Tests
{
    [TestFixture]
    public class Using_Abstract_Automaton
    {
        [Test]
        public void Trigger_is_converted_to_message()
        {
            var busMock = new Mock<IBus>();
            var manualTrigger = new ManualTrigger();
            var messageA = new MessageA();
            var a = new AdHocAutomaton<MessageA>(() => manualTrigger, () => messageA);
            a.Bus = busMock.Object;
            manualTrigger.Trigger();

            busMock.Verify(b=>b.Publish(messageA));
        }
    }
}