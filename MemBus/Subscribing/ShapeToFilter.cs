using System;

namespace MemBus.Subscribing
{

    /// <summary>
    /// Wrap a subscription such that a filter can be specified which pre-filters the messages passed to the inner subscription.
    /// </summary>
    public class ShapeToFilter<M> : ISubscriptionShaper
    {
        private readonly Func<M, bool> filter;

        public ShapeToFilter(Func<M,bool> filter)
        {
            this.filter = filter;
        }

        public ISubscription EnhanceSubscription(ISubscription subscription)
        {
            return new FilteredSubscription<M>(filter, subscription);
        }
    }
}