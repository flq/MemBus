using System;
using System.Threading;

using MemBus.Subscribing;
using MemBus.Support;

#if !WINRT
using System.Windows.Threading;
using Moq;

using MemBus.Publishing;
using MemBus.Setup;
#endif

namespace MemBus.Tests.Frame
{
    internal static class Helpers
    {
        #if !WINRT
        public static Mock<ISubscription> MockSubscriptionThatHandles<T>()
        {
            var mock = new Mock<ISubscription>();
            mock.Setup(m => m.Handles(typeof(T))).Returns(true);
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

        public static PublishChainCasing Configure(this PublishChainCasing pipeline, Action<IConfigurablePublishing> configure)
        {
            configure((IConfigurablePublishing) pipeline);
            return pipeline;
        }
        #endif

        public static SubscriptionBuilder MakeBuilder(this IMethodInfoScanner scanner)
        {
            var b = new SubscriptionBuilder();
            b.AddScanner(scanner);
            return b;
        }

        public static SubscriptionBuilder MakeBuilder(IMethodInfoScanner[] scanner)
        {
            var b = new SubscriptionBuilder();
            scanner.Each(b.AddScanner);
            return b;
        }
    }
}