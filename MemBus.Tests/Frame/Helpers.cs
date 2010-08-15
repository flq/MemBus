using System;
using System.Threading;
using System.Windows.Threading;
using MemBus.Publishing;
using MemBus.Setup;
using MemBus.Support;
using Moq;
using Moq.Protected;

namespace MemBus.Tests.Frame
{
    public static class Helpers
    {
        public static Mock<ISubscription> MockSubscriptionThatHandles<T>()
        {
            var mock = new Mock<ISubscription>();
            mock.Setup(m => m.Handles).Returns(typeof (T));
            return mock;
        }

        public static Mock<T> MockOf<T>() where T : class
        {
            return new Mock<T>(MockBehavior.Loose);
        }

        public static void CreateDispatchContext()
        {
            SynchronizationContext.SetSynchronizationContext(
                new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
        }

        public static PublishPipeline Configure(this PublishPipeline pipeline, Action<IConfigurablePublishing> configure)
        {
            configure((IConfigurablePublishing) pipeline);
            return pipeline;
        }
    }
}