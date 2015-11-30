using System;
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

#if !COREFX
        public static System.Windows.Threading.DispatcherSynchronizationContext CreateDispatchContext()
        {
            var syncContext = new System.Windows.Threading.DispatcherSynchronizationContext(
                System.Windows.Threading.Dispatcher.CurrentDispatcher);
            System.Threading.SynchronizationContext.SetSynchronizationContext(syncContext);
            return syncContext;
        }
#endif

        public static MessageEndpointsBuilder MakeBuilder(this IMethodInfoScanner scanner)
        {
            var b = new MessageEndpointsBuilder();
            b.AddScanner(scanner);
            return b;
        }
    }
}