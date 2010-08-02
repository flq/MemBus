using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MemBus.Subscribing
{

    /// <summary>
    /// Subsequent adds form a matroschka from inner to outer
    /// </summary>
    public class SubscriptionMatroschkaFactory : ISubscriptionShaper, IEnumerable<ISubscriptionShaper>
    {
        readonly List<ISubscriptionShaper> shapers = new List<ISubscriptionShaper>();

        private SubscriptionMatroschkaFactory(IEnumerable<ISubscriptionShaper> shapers)
        {
            this.shapers = new List<ISubscriptionShaper>(shapers);
        }

        public SubscriptionMatroschkaFactory() { }

        public void Add(ISubscriptionShaper shaper)
        {
            shapers.Add(shaper);
        }

        public void AddNextToInner(ISubscriptionShaper shaper)
        {
            addAsFirst(shaper);
        }


        public ISubscription EnhanceSubscription(ISubscription subscription)
        {
            var aggregatedSubscription = 
                shapers.Aggregate(subscription, (current, shaper) => shaper.EnhanceSubscription(current));
            return aggregatedSubscription;
        }

        private void addAsFirst(ISubscriptionShaper shaper)
        {
            if (shapers.Count == 0)
                shapers.Add(shaper);
            else
                shapers.Insert(0, shaper);
        }

        public SubscriptionMatroschkaFactory Clone()
        {
            return new SubscriptionMatroschkaFactory(shapers);
        }

        public IEnumerator<ISubscriptionShaper> GetEnumerator()
        {
            return shapers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}