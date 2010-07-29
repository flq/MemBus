using System;

namespace MemBus.Subscribing
{
    public interface ISubscriptionCustomizer<out M> : ISubscriptionShape
    {
        ISubscriptionCustomizer<M> SetFilter(Func<M, bool> filter);
    }
}