using System.Collections.Generic;

namespace MemBus.Subscribing
{
    public interface ISubscriptionBuilder
    {
        IEnumerable<ISubscription> BuildSubscriptions(object targetToAdapt);
    }
}