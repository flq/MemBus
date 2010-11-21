using System.Collections.Generic;

namespace MemBus.Subscribing
{
    public interface IAdapterServices
    {
        IEnumerable<ISubscription> SubscriptionsFor(object subscriber);
    }
}