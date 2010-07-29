using System;
using Rf.Common;

namespace MemBus.Subscribing
{
    public interface ISubscriptionShape
    {
        ISubscription ConstructSubscription<T>(Action<T> subscription);
    }
}