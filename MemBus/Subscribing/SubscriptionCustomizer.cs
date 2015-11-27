using System;
using System.Threading.Tasks;
using MemBus.Support;

namespace MemBus.Subscribing
{
    /// <summary>
    /// Used by <see cref="ISubscriber.Subscribe{M}(System.Action{M},System.Action{MemBus.Subscribing.SubscriptionCustomizer{M}})"/>
    /// to allow the subscriber to customize the way it obtains messages
    /// </summary>
    /// <typeparam name="M">The message type</typeparam>
    public class SubscriptionCustomizer<M> : ISubscriptionShaper
    {
        private readonly SubscriptionShaperAggregate _subscriptionShaperAggregate;
        private readonly IServices _services;

        private ISubscriptionShaper _filterShape;
        private ISubscriptionShaper _uiInvokeshape;

        internal SubscriptionCustomizer(SubscriptionShaperAggregate subscriptionShaperAggregate, IServices services)
        {
            this._subscriptionShaperAggregate = subscriptionShaperAggregate;
            this._services = services;
        }

        /// <summary>
        /// Specify a filter to be put in front of the subscription.
        /// </summary>
        public SubscriptionCustomizer<M> SetFilter(Func<M, bool> filter)
        {
            _filterShape = new ShapeToFilter<M>(filter);
            return this;
        }

        /// <summary>
        /// Say that this subscription must run on a UI thread
        /// </summary>
        public SubscriptionCustomizer<M> DispatchOnUiThread()
        {
            if (_services.Get<TaskScheduler>() == null)
              throw new InvalidOperationException("No knowledge of a UI thread is available. This method cannot be called. Please setup your bus for a UI scenario with RichClientFrontend");
            _uiInvokeshape = new ShapeToUiDispatch(_services.Get<TaskScheduler>());
            return this;
        }

        ISubscription ISubscriptionShaper.EnhanceSubscription(ISubscription subscription)
        {
            _subscriptionShaperAggregate.AddNextToInner(_filterShape);
            _subscriptionShaperAggregate.AddNextToInner(_uiInvokeshape);
            return _subscriptionShaperAggregate.EnhanceSubscription(subscription);
        }
    }
}