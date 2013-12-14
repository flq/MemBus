using System;
using System.Threading;
using System.Windows.Threading;
using MemBus.Publishing;
using MemBus.Setup;
using MemBus.Subscribing;
using Membus.Tests.Help;

namespace MemBus.Tests.Help
{
    internal static class Helpers
    {
        
        public static ISubscription MockSubscriptionThatHandles<T>()
        {
            return new SubscriptionThatFakesHandles<T>();
        }

        public static PublishChainCasing Configure(this PublishChainCasing pipeline, Action<IConfigurablePublishing> configure)
        {
            configure(pipeline);
            return pipeline;
        }

//        public static Mock<T> MockOf<T>() where T : class
//        {
//            return new Mock<T>(MockBehavior.Loose);
//        }
//
        public static DispatcherSynchronizationContext CreateDispatchContext()
        {
            var syncContext = new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher);
            SynchronizationContext.SetSynchronizationContext(syncContext);
            return syncContext;
        }

//
//        public static PublishChainCasing Configure(this PublishChainCasing pipeline, Action<IConfigurablePublishing> configure)
//        {
//            configure((IConfigurablePublishing) pipeline);
//            return pipeline;
//        }
//        
//
        public static SubscriptionBuilder MakeBuilder(this IMethodInfoScanner scanner)
        {
            var b = new SubscriptionBuilder();
            b.AddScanner(scanner);
            return b;
        }
//
//        public static SubscriptionBuilder MakeBuilder(IMethodInfoScanner[] scanner)
//        {
//            var b = new SubscriptionBuilder();
//            scanner.Each(b.AddScanner);
//            return b;
//        }
    }
}