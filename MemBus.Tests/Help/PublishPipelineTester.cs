using System;
using System.Threading.Tasks;
using MemBus.Publishing;
using MemBus.Setup;

namespace MemBus.Tests.Help
{
    public class AsyncPublishPipelineTester
    {
        public Task TestWith(Action<IConfigurablePublishing> configuration, object message)
        {
            return PipelineSkeleton(message, configuration);
        }
        
        private static Task PipelineSkeleton(object message, Action<IConfigurablePublishing> configuration)
        {
            var p = new PublishChainCasing(null).Configure(configuration);
            var token = new AsyncPublishToken(message, new IAsyncSubscription[] { });
            return p.LookAtAsync(token);
        }
    }
    
    public class PublishPipelineTester<T> where T : new()
    {
        public FakePublishPipelineMember Mock1 { get; }
        public IPublishPipelineMember Mock1Object => Mock1;
        public FakePublishPipelineMember Mock2 { get; }
        public IPublishPipelineMember Mock2Object => Mock2;
        public FakePublishPipelineMember Mock3 { get; }
        public IPublishPipelineMember Mock3Object => Mock3;

        public PublishPipelineTester()
        {
            Mock1 = new FakePublishPipelineMember();
            Mock2 = new FakePublishPipelineMember();
            Mock3 = new FakePublishPipelineMember();
        }

        public void TestWith(Action<IConfigurablePublishing> configuration)
        {
            PipelineSkeleton(new T(), configuration);
        }

        private static void PipelineSkeleton(object message, Action<IConfigurablePublishing> configuration)
        {
            var p = new PublishChainCasing(null).Configure(configuration);
            var token = new PublishToken(message, new ISubscription[] { });
            p.LookAt(token);
        }

    }

    public class FakePublishPipelineMember : IPublishPipelineMember
    {
        private bool _cancelToken;

        public void LookAt(PublishToken token)
        {
            Token = token;
            if (_cancelToken)
                Token.Cancel = true;
        }

        public void VerifyNotCalled()
        {
            Token.ShouldBeNull();
        }

        public void VerifyCalled()
        {
            Token.ShouldNotBeNull();
        }

        public PublishToken Token { get; set; }

        internal void CancelTokenWhenSeen()
        {
            _cancelToken = true;
        }
    }
}