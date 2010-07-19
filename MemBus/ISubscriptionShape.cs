using System;

namespace MemBus
{
    public interface ISubscriptionShape
    {
        ISubscription ConstructSubscription<T>(Action<T> subscription);
    }
}