using System;
using System.Threading;
using MemBus.Tests.Help;
using Moq;
using NUnit.Framework;
using System.Linq;
using MemBus.Tests.Frame;

namespace MemBus.Tests
{
    [TestFixture]
    public class Using_Publish_Pipeline
    {
        [Test]
        public void publishes_message_parallel()
        {
            var p = new ParallelBlockingPublisher();
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
            var p = new ParallelNonBlockingPublisher();
            var evtBlock = new ManualResetEvent(false);
            var evtSignal = new ManualResetEvent(false);
            var evtSignal2 = new ManualResetEvent(false);
            var lockingSub = new MockSubscription<MessageA>(evtBlock, evtSignal);
            var runThroughSub = new MockSubscription<MessageA>(evtSignal:evtSignal2);

            var token = new PublishToken(new MessageA(), new[] { lockingSub, runThroughSub });
            p.LookAt(token);
            lockingSub.Received.ShouldBeEqualTo(0);
            evtSignal2.WaitOne();
            runThroughSub.Received.ShouldBeEqualTo(1);
            evtBlock.Set();
            evtSignal.WaitOne();
            lockingSub.Received.ShouldBeEqualTo(1);
        }

        [Test]
        public void publish_pipeline_is_extensible()
        {
            var m1 = new Mock<IPublishPipelineMember>(MockBehavior.Loose);
            var m2 = new Mock<IPublishPipelineMember>(MockBehavior.Loose);

            var p = new PublishPipeline { m1.Object, m2.Object };
            var token = new PublishToken(new MessageA(), new ISubscription[] {  });
            p.LookAt(token);
            m1.Verify(m=>m.LookAt(token));
            m2.Verify(m => m.LookAt(token));
        }
    }
}