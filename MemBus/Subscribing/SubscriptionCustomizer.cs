using System;
using System.Threading.Tasks;
using MemBus.Support;

namespace MemBus.Subscribing
{
    internal class SubscriptionCustomizer<M> : ISubscriptionCustomizer<M>
    {
        private readonly SubscriptionShaperAggregate subscriptionShaperAggregate;
        private readonly IServices services;

        private ISubscriptionShaper filterShape;
        private ISubscriptionShaper uiInvokeshape;

        public SubscriptionCustomizer(SubscriptionShaperAggregate subscriptionShaperAggregate, IServices services)
        {
            this.subscriptionShaperAggregate = subscriptionShaperAggregate;
            this.services = services;
        }

        public ISubscriptionCustomizer<M> SetFilter(Func<M, bool> filter)
        {
            filterShape = new ShapeToFilter<M>(filter);
            return this;
        }

        public ISubscriptionCustomizer<M> DispatchOnUiThread()
        {
            if (services.Get<TaskScheduler>() == null)
              throw new InvalidOperationException("No knowledge of a UI thread is available. This method cannot be called. Please setup your bus for a UI scenario with RichClientFrontend");
            uiInvokeshape = new ShapeToUiDispatch(services.Get<TaskScheduler>());
            return this;
        }

        public ISubscription EnhanceSubscription(ISubscription subscription)
        {
            subscriptionShaperAggregate.AddNextToInner(filterShape);
            subscriptionShaperAggregate.AddNextToInner(uiInvokeshape);
            return subscriptionShaperAggregate.EnhanceSubscription(subscription);
        }
    }
}