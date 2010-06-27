using System;
using System.Collections.Generic;

namespace MemBus
{
    public class TableBasedResolver : ISubscriptionResolver
    {
        public IEnumerable<ISubscription> GetSubscriptionsFor(object message)
        {
            throw new NotImplementedException();
        }
    }
}