using MemBus.Subscribing;
using MemBus.Tests.Help;
using Moq;
using NUnit.Framework;
using MemBus.Tests.Frame;

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
                .StartWith<Default>(cb => cb.AddSubscription(sub.Object))
                .Construct();
            var messageA = new MessageA();
            b.Publish(messageA);
            sub.Verify(s=>s.Push(messageA));
        }

        [Test]
        public void Default_setup_provides_subscription_shape()
        {
            var received = 0;
            var b = BusSetup.StartWith<Default>().Construct();
            var d = b.Subscribe<MessageA>(msg => received++);
            b.Publish(new MessageA());
            received.ShouldBeEqualTo(1);
        }

        [Test]
        public void Resolvers_will_get_access_to_services()
        {
            var simpleResolver = new SimpleResolver();
            var b = BusSetup.StartWith<Default>(cb=> cb.InsertResolver(simpleResolver)).Construct();
            simpleResolver.Services.ShouldNotBeNull();
        }
        
    }
}