using System.Threading;
using MemBus.Tests.Help;
using NUnit.Framework;
using System.Linq;
using MemBus.Tests.Frame;

namespace MemBus.Tests
{
    [TestFixture]
    public class Using_Publish_Pipeline_Members
    {
        [Test]
        public void publishes_message_parallel()
        {
            var p = new ParallelPublisher();
            publisherCheck(p);
        }

        [Test]
        public void publishes_message_sequentially()
        {
            var p = new SequentialPublisher();
            publisherCheck(p);
        }

        private static void publisherCheck(IPublishPipelineMember p)
        {
            var token = new PublishToken(new MessageA(), new[] { new MockSubscription<MessageA>(), new MockSubscription<MessageA>() });
            p.LookAt(token);
            token.Subscriptions.OfType<MockSubscription<MessageA>>().All(s=>s.Received == 1).ShouldBeTrue();
        }

        [Test]
        public void publishes_message_fire_and_forget()
        {
            var p = new FireAndForgetPublisher();
            var resetEvt1 = new ManualResetEvent(false);
            var lockingSub = new MockSubscription<MessageA>(resetEvt1);
            var runThroughSub = new MockSubscription<MessageA>();

            var token = new PublishToken(new MessageA(), new[] { lockingSub, runThroughSub });
            p.LookAt(token);
            
            lockingSub.Received.ShouldBeEqualTo(0);
            runThroughSub.Received.ShouldBeEqualTo(1);
            resetEvt1.Set();
            Thread.Sleep(300);
            lockingSub.Received.ShouldBeEqualTo(1);
        }
    }
}