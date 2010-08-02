using System;
using Rf.Common;

namespace MemBus.Subscribing
{
    public class SubscriptionCustomizer<M> : ISubscriptionCustomizer<M>
    {
        private readonly SubscriptionMatroschkaFactory subscriptionMatroschka;

        public SubscriptionCustomizer(SubscriptionMatroschkaFactory subscriptionMatroschka)
        {
            this.subscriptionMatroschka = subscriptionMatroschka;
        }

        public ISubscriptionCustomizer<M> SetFilter(Func<M, bool> filter)
        {
            subscriptionMatroschka.AddNextToInner(new ShapeToFilter<M>(filter));
            return this;
        }

        public ISubscription EnhanceSubscription(ISubscription subscription)
        {
            return subscriptionMatroschka.EnhanceSubscription(subscription);
        }
    }
}