using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MemBus.Subscribing
{

    /// <summary>
    /// Subsequent adds form a matroschka from inner to outer
    /// </summary>
    internal class SubscriptionShaperAggregate : ISubscriptionShaper, IEnumerable<ISubscriptionShaper>
    {
        readonly List<ISubscriptionShaper> _shapers = new List<ISubscriptionShaper>();

        public SubscriptionShaperAggregate(IEnumerable<ISubscriptionShaper> shapers)
        {
            _shapers = new List<ISubscriptionShaper>(shapers);
        }

        public SubscriptionShaperAggregate() { }

        public void Add(ISubscriptionShaper shaper)
        {
            _shapers.Add(shaper);
        }

        /// <summary>
        /// Add a shape next to the inner one. It is allowed to pass null. In this case, nothing is added
        /// </summary>
        public void AddNextToInner(ISubscriptionShaper shaper)
        {
            if (shaper != null)
              addAsFirst(shaper);
        }


        public ISubscription EnhanceSubscription(ISubscription subscription)
        {
            var aggregatedSubscription = 
                _shapers.Aggregate(subscription, (current, shaper) => shaper.EnhanceSubscription(current));
            return aggregatedSubscription;
        }

        private void addAsFirst(ISubscriptionShaper shaper)
        {
            if (_shapers.Count == 0)
                _shapers.Add(shaper);
            else
                _shapers.Insert(0, shaper);
        }

        public SubscriptionShaperAggregate Clone()
        {
            return new SubscriptionShaperAggregate(_shapers);
        }

        public IEnumerator<ISubscriptionShaper> GetEnumerator()
        {
            return _shapers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}