using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MemBus
{
    /// <summary>
    /// Pure aggregation logic of subscription without 
    /// checking whether the subscriptions can handle the message that is entering
    /// </summary>
    public class SubscriptionAggregation : IEnumerable<ISubscription>
    {
        private readonly List<IEnumerable<ISubscription>> subscriptionSets = new List<IEnumerable<ISubscription>>();

        public SubscriptionAggregation(params IEnumerable<ISubscription>[] subscriptionSets)
        {
            this.subscriptionSets.AddRange(subscriptionSets);
        }

        public SubscriptionAggregation()
        {
            
        }

        public void Add(IEnumerable<ISubscription> subscriptionSet)
        {
            subscriptionSets.Add(subscriptionSet);
        }

        public IEnumerator<ISubscription> GetEnumerator()
        {
            return subscriptionSets.SelectMany(set => set).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}