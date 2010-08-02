using System;
using Rf.Common;

namespace MemBus.Subscribing
{
    public interface ISubscriptionShaper
    {
        ISubscription EnhanceSubscription(ISubscription subscription);
    }
}