using System;
using System.Collections.Generic;

namespace MemBus.Subscribing
{
    public interface IAdapterServices
    {
        IDisposable WireUpSubscriber(ISubscriptionResolver subscriptionResolver, object subscriber);
    }
}