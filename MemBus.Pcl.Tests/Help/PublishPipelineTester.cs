using System;
using MemBus.Publishing;
using MemBus.Setup;

namespace MemBus.Tests.Help
{
    public class PublishPipelineTester<T> where T : new()
    {
        private PublishToken _token;

        public FakePublishPipelineMember Mock1 { get; private set; }
        public IPublishPipelineMember Mock1Object { get { return Mock1; } }
        public FakePublishPipelineMember Mock2 { get; private set; }
        public IPublishPipelineMember Mock2Object { get { return Mock2; } }
        public FakePublishPipelineMember Mock3 { get; private set; }
        public IPublishPipelineMember Mock3Object { get { return Mock3; } }

        public PublishPipelineTester()
        {
            Mock1 = new FakePublishPipelineMember();
            Mock2 = new FakePublishPipelineMember();
            Mock3 = new FakePublishPipelineMember();
        }

        public PublishPipelineTester<T> TestWith(Action<IConfigurablePublishing> configuration)
        {
            _token = pipelineSkeleton(new T(), configuration);
            return this;
        }

        private static PublishToken pipelineSkeleton(object message, Action<IConfigurablePublishing> configuration)
        {
            var p = new PublishChainCasing(null).Configure(configuration);
            var token = new PublishToken(message, new ISubscription[] { });
            p.LookAt(token);
            return token;
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