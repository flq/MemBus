using System.Collections.Generic;

namespace MemBus.Subscribing
{
    internal interface ISubscriptionBuilder
    {
        IEnumerable<ISubscription> BuildSubscriptions(object targetToAdapt);
    }
}