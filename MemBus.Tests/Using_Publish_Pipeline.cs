using System;
using System.Threading;
using MemBus.Configurators;
using MemBus.Publishing;
using MemBus.Support;
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
            var t = new PublishPipelineTester<MessageB>();
            t.TestWith(pp => pp.DefaultPublishPipeline(t.Mock1Object, t.Mock2Object));

            t.VerifyCalled(t.Mock1);
            t.VerifyCalled(t.Mock2);
        }

        [Test]
        public void non_default_publish_pipeline_takes_precedence()
        {
            var t = new PublishPipelineTester<MessageA>();
            t.TestWith(
                pp =>
                    {
                        pp.DefaultPublishPipeline(t.Mock1Object, t.Mock2Object);
                        pp.MessageMatch(mi => mi.IsType<MessageA>(), cp => cp.PublishPipeline(t.Mock2Object));
                    });


            t.VerifyNotCalled(t.Mock1);
            t.VerifyCalled(t.Mock2);
        }

        [Test]
        public void default_publish_pipeline_is_fallback()
        {
            var t = new PublishPipelineTester<MessageA>();
            t.TestWith(pp =>
                           {
                               pp.DefaultPublishPipeline(t.Mock1Object, t.Mock3Object);
                               pp.MessageMatch(mi => mi.IsType<MessageB>(),
                                                     cp => cp.PublishPipeline(t.Mock2Object));
                           });


            t.VerifyCalled(t.Mock1);
            t.VerifyNotCalled(t.Mock2);
            t.VerifyCalled(t.Mock3);
        }

        [Test]
        public void use_send_this_to_send_message_while_publishing()
        {
            var b = BusSetup.StartWith<Conservative>(cb=> cb.ConfigurePublishing(
                p=> p.MessageMatch(
                    m => m.IsType<MessageA>(),
                    c => c.PublishPipeline(Publish.This(new MessageB()), new SequentialPublisher()))))
                .Construct();

            int bCount = 0;
            int aCount = 0;
            b.Subscribe<MessageB>(msg => bCount++);
            b.Subscribe<MessageA>(msg => aCount++);

            b.Publish(new MessageA());
            bCount.ShouldBeEqualTo(1);
            aCount.ShouldBeEqualTo(1);
        }


    }
}