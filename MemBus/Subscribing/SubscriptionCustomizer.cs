using System;
using System.Threading.Tasks;
using MemBus.Support;

namespace MemBus.Subscribing
{
    /// <summary>
    /// Used by <see cref="ISubscriber.Subscribe{M}(System.Action{M},System.Action{MemBus.Subscribing.SubscriptionCustomizer{M}})"/>
    /// to allow the subscriber to customize the way it obtains messages
    /// </summary>
    /// <typeparam name="M"></typeparam>
    public class SubscriptionCustomizer<M> : ISubscriptionShaper
    {
        private readonly SubscriptionShaperAggregate subscriptionShaperAggregate;
        private readonly IServices services;

        private ISubscriptionShaper filterShape;
        private ISubscriptionShaper uiInvokeshape;

        internal SubscriptionCustomizer(SubscriptionShaperAggregate subscriptionShaperAggregate, IServices services)
        {
            this.subscriptionShaperAggregate = subscriptionShaperAggregate;
            this.services = services;
        }

        public SubscriptionCustomizer<M> SetFilter(Func<M, bool> filter)
        {
            filterShape = new ShapeToFilter<M>(filter);
            return this;
        }

        public SubscriptionCustomizer<M> DispatchOnUiThread()
        {
            if (services.Get<TaskScheduler>() == null)
              throw new InvalidOperationException("No knowledge of a UI thread is available. This method cannot be called. Please setup your bus for a UI scenario with RichClientFrontend");
            uiInvokeshape = new ShapeToUiDispatch(services.Get<TaskScheduler>());
            return this;
        }

        ISubscription ISubscriptionShaper.EnhanceSubscription(ISubscription subscription)
        {
            subscriptionShaperAggregate.AddNextToInner(filterShape);
            subscriptionShaperAggregate.AddNextToInner(uiInvokeshape);
            return subscriptionShaperAggregate.EnhanceSubscription(subscription);
        }
    }
}