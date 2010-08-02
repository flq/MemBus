using System;

namespace MemBus.Subscribing
{
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