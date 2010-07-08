using Moq;
using NUnit.Framework;

namespace MemBus.Tests
{
    [TestFixture]
    public class When_using_the_bus
    {
        [Test]
        public void Default_setup_routes_the_message_correctly()
        {
            var sub = new Mock<ISubscription>();
            sub.Setup(s => s.Handles).Returns(typeof (MessageA));
            var b = BusSetup
                .StartWith<DefaultSetup>(new AdHocConfigurator(cb => cb.AddSubscription(sub.Object)))
                .Construct();
            var messageA = new MessageA();
            b.Publish(messageA);
            sub.Verify(s=>s.Push(messageA));
        }
        
    }
}