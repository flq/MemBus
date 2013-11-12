using System;

namespace MemBus.Subscribing
{
    public interface ISubscriptionShaper
    {
        ISubscription EnhanceSubscription(ISubscription subscription);
    }
}