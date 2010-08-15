using System;

namespace MemBus.Subscribing
{
    public class ShapeToPassthrough : ISubscriptionShaper
    {
        public ISubscription EnhanceSubscription(ISubscription subscription)
        {
            return subscription;
        }
    }
}