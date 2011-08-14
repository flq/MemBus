using System;
using System.Threading.Tasks;
using MemBus.Configurators;

namespace MemBus.Subscribing
{
    /// <summary>
    /// Used by <see cref="ISubscriber.Subscribe{M}(System.Action{M},System.Action{MemBus.Subscribing.ISubscriptionCustomizer{M}})"/>
    /// to allow the subscriber to customaize the way it obtains messages
    /// </summary>
    /// <typeparam name="M"></typeparam>
    public interface ISubscriptionCustomizer<out M> : ISubscriptionShaper
    {
        /// <summary>
        /// Set a filter to only obtain those messages that pass the specified filter
        /// </summary>
        ISubscriptionCustomizer<M> SetFilter(Func<M, bool> filter);

        /// <summary>
        /// Say that you want to Dispatch message reception on the UI-thread. You need to have added a <see cref="TaskScheduler"/> from the method
        /// <see cref="TaskScheduler.FromCurrentSynchronizationContext"/> to the MemBus Services as is done in e.g. <see cref="RichClientFrontend"/>
        /// </summary>
        ISubscriptionCustomizer<M> DispatchOnUiThread();
    }
}