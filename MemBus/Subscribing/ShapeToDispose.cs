namespace MemBus.Subscribing
{
    public class ShapeToDispose : ISubscriptionShaper
    {
        public ISubscription EnhanceSubscription(ISubscription subscription)
        {
            return subscription is IDisposableSubscription ? subscription : new DisposableSubscription(subscription);
        }
    }
}