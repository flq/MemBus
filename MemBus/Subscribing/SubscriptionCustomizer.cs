using System;
using System.Threading.Tasks;
using Rf.Common;

namespace MemBus.Subscribing
{
    public class SubscriptionCustomizer<M> : ISubscriptionCustomizer<M>
    {
        private readonly SubscriptionMatroschkaFactory subscriptionMatroschka;
        private readonly IServices services;

        private ISubscriptionShaper filterShape;
        private ISubscriptionShaper uiInvokeshape;

        public SubscriptionCustomizer(SubscriptionMatroschkaFactory subscriptionMatroschka, IServices services)
        {
            this.subscriptionMatroschka = subscriptionMatroschka;
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
            subscriptionMatroschka.AddNextToInner(filterShape);
            subscriptionMatroschka.AddNextToInner(uiInvokeshape);
            return subscriptionMatroschka.EnhanceSubscription(subscription);
        }
    }
}