using System;
using System.Reactive.Linq;
using System.Threading;
using MemBus.Configurators;
using MemBus.Messages;
using MemBus.Tests.Help;
using Membus.Tests.Help;

using Xunit;

namespace MemBus.Tests.Integration
{
    
    public class When_Using_The_Bus
    {
        [Fact]
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

        [Fact]
        public void Default_setup_provides_subscription_shape()
        {
            var received = 0;
            var b = BusSetup.StartWith<Conservative>().Construct();
            using (b.Subscribe<MessageA>(msg => received++))
            {
                b.Publish(new MessageA());
                received.ShouldBeEqualTo(1);
            }
        }

        [Fact]
        public void Resolvers_will_get_access_to_services()
        {
            var simpleResolver = new SimpleResolver();
            using (BusSetup.StartWith<Conservative>(cb => cb.AddResolver(simpleResolver)).Construct())
            {
                simpleResolver.Services.ShouldNotBeNull();
            }
        }

        
        [Fact]
        public void Subscription_with_filtering_works()
        {
            var received = 0;
            var b = BusSetup.StartWith<Conservative>().Construct();
            using (b.Observe<MessageB>().Where(msg => msg.Id == "A").Subscribe(msg => received++))
            {
                b.Publish(new MessageB { Id = "A" });
                b.Publish(new MessageB { Id = "B" });
            }
            received.ShouldBeEqualTo(1);
        }
       

        [Fact]
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
                Assert.True(false, "Exception was never captured!");

            capturedMessage.Exception.ShouldBeOfType<AggregateException>();
            var xes = ((AggregateException) capturedMessage.Exception).InnerExceptions;
            xes[0].ShouldBeOfType<ArgumentException>();
            xes[0].Message.ShouldBeEqualTo("Bad message");
        }

        [Fact]
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