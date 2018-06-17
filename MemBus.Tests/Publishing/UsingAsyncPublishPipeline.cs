using System;
using System.Threading.Tasks;
using MemBus.Configurators;
using MemBus.Publishing;
using MemBus.Support;
using MemBus.Tests.Help;
using FakeItEasy;
using Xunit;

namespace MemBus.Tests.Publishing
{
    public class UsingAsyncPublishPipeline
    {
        [Fact]
        public async Task awaits_all_subscriptions()
        {
            var p = new WhenAllTaskPublisher();
            await PublisherCheck(p);
        }

        [Fact]
        public async Task publishes_message_sequentially()
        {
            var p = new SequentialPublisher();
            await PublisherCheck(p);
        }

        private async Task PublisherCheck(IAsyncPublishPipelineMember p)
        {
            var subs = new[] {CreateAsyncSub(), CreateAsyncSub()};
            var token = new AsyncPublishToken(new MessageA(), subs);
            await p.LookAtAsync(token);
            A.CallTo(() => subs[0].PushAsync(A<object>.Ignored)).MustHaveHappened();
            A.CallTo(() => subs[1].PushAsync(A<object>.Ignored)).MustHaveHappened();
        }        

        [Fact]
        public async Task publish_pipeline_is_extensible()
        {
            var (member1, member2) = (CreatePublishPipelineMember(), CreatePublishPipelineMember());
            var t = new AsyncPublishPipelineTester();
            await t.TestWith(
                pp => pp.DefaultAsyncPublishPipeline(member1, member2),
                new MessageA());

            A.CallTo(() => member1.LookAtAsync(A<AsyncPublishToken>.Ignored))
                .MustHaveHappened();
            A.CallTo(() => member2.LookAtAsync(A<AsyncPublishToken>.Ignored))
                .MustHaveHappened();
        }

		[Fact]
		public async Task default_pubish_pipeline_is_replaceable()
		{
		    var m1 = CreatePublishPipelineMember();
		    var (m2, m3) = (CreatePublishPipelineMember(), CreatePublishPipelineMember());
		    var t = new AsyncPublishPipelineTester();
		    await t.TestWith(cfg => cfg
		        .DefaultAsyncPublishPipeline(m1)
		        .DefaultAsyncPublishPipeline(m2, m3), new MessageA());
		    
		    A.CallTo(() => m1.LookAtAsync(A<AsyncPublishToken>.Ignored)).MustNotHaveHappened();
		    A.CallTo(() => m2.LookAtAsync(A<AsyncPublishToken>.Ignored)).MustHaveHappened();
		    A.CallTo(() => m3.LookAtAsync(A<AsyncPublishToken>.Ignored)).MustHaveHappened();
		}

        [Fact]
        public async Task non_default_publish_pipeline_takes_precedence()
        {
            var (m1, m2) = (CreatePublishPipelineMember(), CreatePublishPipelineMember());
            var t = new AsyncPublishPipelineTester();
            await t.TestWith(
                pp =>
                {
                    pp.DefaultAsyncPublishPipeline(m1);
                    pp.MessageMatch(mi => mi.IsType<MessageA>())
                        .AsyncPublishPipeline(m2);
                }, new MessageA());

            A.CallTo(() => m1.LookAtAsync(A<AsyncPublishToken>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => m2.LookAtAsync(A<AsyncPublishToken>.Ignored)).MustHaveHappened();
        }

        [Fact(Skip = "Not ready")]
        public void Execution_of_pipeline_is_cancellable_by_member()
        {
            var t = new PublishPipelineTester<MessageA>();
            t.Mock1.CancelTokenWhenSeen();
            t.TestWith(pp => pp.DefaultPublishPipeline(t.Mock1Object, t.Mock2Object));
            t.Mock1.VerifyCalled();
            t.Mock2.VerifyNotCalled();
        }

        [Fact(Skip = "Not ready")]
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

        [Fact(Skip = "Not ready")]
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

        [Fact(Skip = "Not ready")]
        public void MessageInfo_isType_supports_variance()
        {
            var info = new MessageInfo(new Foo());
            info.IsType<Base>().ShouldBeTrue();
        }
        
        IAsyncPublishPipelineMember CreatePublishPipelineMember() 
            => A.Fake<IAsyncPublishPipelineMember>();

        private IAsyncSubscription CreateAsyncSub()
        {
            var sub = A.Fake<IAsyncSubscription>();
            A.CallTo(() => sub.Handles(A<Type>.Ignored)).Returns(true);
            return sub;
        }

        class Base { }

        class Foo : Base { }
    }
}