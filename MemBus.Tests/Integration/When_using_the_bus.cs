using System;
using System.Threading;
using MemBus.Configurators;
using MemBus.Messages;
using MemBus.Tests.Help;
using MemBus.Tests.Frame;
using Membus.Tests.Help;
#if WINRT
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
using TestFixtureSetUp = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestInitializeAttribute;
#else
using NUnit.Framework;
#endif

namespace MemBus.Tests.Integration
{
    [TestFixture]
    public class When_using_the_bus
    {
        [Test]
        public void Default_setup_routes_the_message_correctly()
        {
            var sub = new SubscriptionThatFakesHandles<MessageA>();
            
            var b = BusSetup
                .StartWith<Conservative>(cb => cb.AddSubscription(sub))
                .Construct();
            var messageA = new MessageA();
            b.Publish(messageA);
            sub.PushCalls.ShouldBeEqualTo(1);
        }

        [Test]
        public void Default_setup_provides_subscription_shape()
        {
            var received = 0;
            var b = BusSetup.StartWith<Conservative>().Construct();
            var d = b.Subscribe<MessageA>(msg => received++);
            b.Publish(new MessageA());
            received.ShouldBeEqualTo(1);
        }

        [Test]
        public void Resolvers_will_get_access_to_services()
        {
            var simpleResolver = new SimpleResolver();
            var b = BusSetup.StartWith<Conservative>(cb=> cb.AddResolver(simpleResolver)).Construct();
            simpleResolver.Services.ShouldNotBeNull();
        }

        #if !WINRT
        [Test]
        public void Subscription_with_filtering_works()
        {
            var received = 0;
            var b = BusSetup.StartWith<Conservative>().Construct();
            b.Subscribe<MessageB>(msg => received++, c=>c.SetFilter(msg=>msg.Id == "A"));
            b.Publish(new MessageB { Id = "A" });
            b.Publish(new MessageB { Id = "B" });
            received.ShouldBeEqualTo(1);
        }
        #endif

        [Test]
        public void Exceptions_are_made_available_as_messages()
        {
            var evt = new ManualResetEvent(false);
            ExceptionOccurred capturedMessage = null;
            var bus = BusSetup.StartWith<AsyncConfiguration>().Construct();
            bus.Subscribe<MessageB>(msg =>
                                        {
                                            throw new ArgumentException("Bad message");
                                        });
            bus.Subscribe<ExceptionOccurred>(x =>
                                                 {
                                                     capturedMessage = x;
                                                     evt.Set();
                                                 });
            
            bus.Publish(new MessageB());
            var signaled = evt.WaitOne(TimeSpan.FromSeconds(2));
            if (!signaled)
                Assert.Fail("Exception was never captured!");

            capturedMessage.Exception.ShouldBeOfType<AggregateException>();
            var xes = ((AggregateException) capturedMessage.Exception).InnerExceptions;
            xes[0].ShouldBeOfType<ArgumentException>();
            xes[0].Message.ShouldBeEqualTo("Bad message");
        }

        [Test]
        public void A_disposed_subscription_is_gone()
        {
            int received = 0;
            var bus = BusSetup.StartWith<Conservative>().Construct();
            var d = bus.Subscribe<MessageA>(msg => received++);
            bus.Publish(new MessageA());
            received.ShouldBeEqualTo(1);
            d.Dispose();
            bus.Publish(new MessageA());
            received.ShouldBeEqualTo(1);
        }

    }
}