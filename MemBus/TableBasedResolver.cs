using System;
using System.Collections.Generic;

namespace MemBus
{
    public class TableBasedResolver : ISubscriptionResolver
    {
        private Dictionary<Type, CompositeSubscription> subscriptions = new Dictionary<Type, CompositeSubscription>();

        public IEnumerable<ISubscription> GetSubscriptionsFor(object message)
        {
            throw new NotImplementedException();
        }

        public bool Add(ISubscription subscription)
        {
            throw new NotImplementedException();
        }
    }
}