using System;
using Moq;

namespace MemBus.Tests.Frame
{
    public class PublishPipelineTester<T> where T : new()
    {
        private PublishToken token;

        public Mock<IPublishPipelineMember> Mock1 { get; private set; }
        public IPublishPipelineMember Mock1Object { get { return Mock1.Object; } }
        public Mock<IPublishPipelineMember> Mock2 { get; private set; }
        public IPublishPipelineMember Mock2Object { get { return Mock2.Object; } }
        public Mock<IPublishPipelineMember> Mock3 { get; private set; }
        public IPublishPipelineMember Mock3Object { get { return Mock3.Object; } }

        public PublishPipelineTester()
        {
            Mock1 = Helpers.MockOf<IPublishPipelineMember>();
            Mock2 = Helpers.MockOf<IPublishPipelineMember>();
            Mock3 = Helpers.MockOf<IPublishPipelineMember>();
        }

        public PublishPipelineTester<T> TestWith(Action<IConfigurablePublishing> configuration)
        {
            token = pipelineSkeleton(new T(), configuration);
            return this;
        }

        private static PublishToken pipelineSkeleton(object message, Action<IConfigurablePublishing> configuration)
        {
            var p = new PublishPipeline(null).Configure(configuration);
            var token = new PublishToken(message, new ISubscription[] { });
            p.LookAt(token);
            return token;
        }

        public void VerifyNotCalled(Mock<IPublishPipelineMember> mock)
        {
            mock.Verify(m => m.LookAt(token), Times.Never());
        }

        public void VerifyCalled(Mock<IPublishPipelineMember> mock)
        {
            mock.Verify(m => m.LookAt(token));
        }
    }
}