using System.Threading;
using MemBus.Configurators;
using MemBus.Publishing;
using MemBus.Support;
using MemBus.Tests.Help;
using System.Linq;

using NUnit.Framework;

namespace MemBus.Tests.Publishing
{
    [TestFixture]
    public class Using_Publish_Pipeline
    {
        [Test]
        public void publishes_message_parallel()
        {
            var p = new ParallelBlockingPublisher();
            PublisherCheck(p);
        }

        [Test]
        public void publishes_message_sequentially()
        {
            var p = new SequentialPublisher();
            PublisherCheck(p);
        }

        private static void PublisherCheck(IPublishPipelineMember p)
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

            t.Mock1.VerifyCalled();
            t.Mock2.VerifyCalled();
        }

		[Test]
		public void default_pubish_pipeline_is_replaceable() 
		{
			var t = new PublishPipelineTester<MessageB>();
			var b = BusSetup.StartWith<Conservative>()
				.Apply(cb => cb.ConfigurePublishing(cp => cp.DefaultPublishPipeline(t.Mock1Object)))
				.Construct();

			b.Publish(new MessageB());

			t.Mock1.VerifyCalled();

		}

        [Test]
        public void non_default_publish_pipeline_takes_precedence()
        {
            var t = new PublishPipelineTester<MessageA>();
            t.TestWith(
                pp =>
                    {
                        pp.DefaultPublishPipeline(t.Mock1Object, t.Mock2Object);
                        pp.MessageMatch(mi => mi.IsType<MessageA>()).PublishPipeline(t.Mock2Object);
                    });


            t.Mock1.VerifyNotCalled();
            t.Mock2.VerifyCalled();
        }

        [Test]
        public void Execution_of_pipeline_is_cancellable_by_member()
        {
            var t = new PublishPipelineTester<MessageA>();
            t.Mock1.CancelTokenWhenSeen();
            t.TestWith(pp => pp.DefaultPublishPipeline(t.Mock1Object, t.Mock2Object));
            t.Mock1.VerifyCalled();
            t.Mock2.VerifyNotCalled();
        }

        [Test]
        public void default_publish_pipeline_is_fallback()
        {
            var t = new PublishPipelineTester<MessageA>();
            t.TestWith(pp =>
                           {
                               pp.DefaultPublishPipeline(t.Mock1Object, t.Mock3Object);
                               pp.MessageMatch(mi => mi.IsType<MessageB>()).PublishPipeline(t.Mock2Object);
                           });


            t.Mock1.VerifyCalled();
            t.Mock2.VerifyNotCalled();
            t.Mock3.VerifyCalled();
        }

        [Test]
        public void use_send_this_to_send_message_while_publishing()
        {
            var b = BusSetup.StartWith<Conservative>(cb=> cb.ConfigurePublishing(
                p => p.MessageMatch(m => m.IsType<MessageA>()).PublishPipeline(Publish.This(new MessageB()), new SequentialPublisher())))
                .Construct();

            int bCount = 0;
            int aCount = 0;
            b.Subscribe<MessageB>(msg => bCount++);
            b.Subscribe<MessageA>(msg => aCount++);

            b.Publish(new MessageA());
            bCount.ShouldBeEqualTo(1);
            aCount.ShouldBeEqualTo(1);
        }

        [Test]
        public void MessageInfo_isType_supports_variance()
        {
            var info = new MessageInfo(new Foo());
            info.IsType<Base>().ShouldBeTrue();
        }

        class Base { }

        class Foo : Base { }
    }
}