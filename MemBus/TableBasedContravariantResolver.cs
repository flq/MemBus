using System;
using System.Collections.Generic;

namespace MemBus
{
    internal class TableBasedContravariantResolver : ISubscriptionResolver
    {
        private readonly Dictionary<Type, CompositeSubscription> subscriptions = new Dictionary<Type, CompositeSubscription>();

        public IEnumerable<ISubscription> GetSubscriptionsFor(object message)
        {
            var a = new SubscriptionAggregation();
            return aggregateSubscriptions(a, message.GetType());
        }

        private IEnumerable<ISubscription> aggregateSubscriptions(SubscriptionAggregation subscriptionAggregation, Type lookupType)
        {
            if (lookupType == null)
                return subscriptionAggregation;
            if (subscriptions.ContainsKey(lookupType))
                subscriptionAggregation.Add(subscriptions[lookupType]);
            return aggregateSubscriptions(subscriptionAggregation, lookupType.BaseType);
        }

        public bool Add(ISubscription subscription)
        {
            var cs = getRelevantComposite(subscription.Handles);
            cs.Add(subscription);
            return true;
        }

        private CompositeSubscription getRelevantComposite(Type messageType)
        {
            CompositeSubscription cs;
            subscriptions.TryGetValue(messageType, out cs);
            return cs ?? (subscriptions[messageType] = new CompositeSubscription());
        }
    }
}