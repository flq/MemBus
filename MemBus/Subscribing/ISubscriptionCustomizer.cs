using System;

namespace MemBus.Subscribing
{
    public interface ISubscriptionCustomizer<out M> : ISubscriptionShaper
    {
        ISubscriptionCustomizer<M> SetFilter(Func<M, bool> filter);
        ISubscriptionCustomizer<M> DispatchOnUiThread();
    }
}