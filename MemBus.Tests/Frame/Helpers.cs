using System;
using System.Threading;

using MemBus.Subscribing;
using MemBus.Support;
using Membus.Tests.Help;

#if !WINRT
using System.Windows.Threading;
using Moq;
#endif

using MemBus.Publishing;
using MemBus.Setup;


namespace MemBus.Tests.Frame
{
    internal static class Helpers
    {
        
        public static ISubscription MockSubscriptionThatHandles<T>()
        {
            return new SubscriptionThatFakesHandles<T>();
        }

        #if !WINRT
        public static Mock<T> MockOf<T>() where T : class
        {
            return new Mock<T>(MockBehavior.Loose);
        }

        public static void CreateDispatchContext()
        {
            SynchronizationContext.SetSynchronizationContext(
                new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
        }
        #endif
        public static PublishChainCasing Configure(this PublishChainCasing pipeline, Action<IConfigurablePublishing> configure)
        {
            configure((IConfigurablePublishing) pipeline);
            return pipeline;
        }
        

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