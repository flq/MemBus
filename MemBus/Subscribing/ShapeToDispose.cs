namespace MemBus.Subscribing
{
    public class ShapeToDispose : ISubscriptionShaper
    {
        public ISubscription EnhanceSubscription(ISubscription subscription)
        {
            return new DisposableSubscription(subscription);
        }
    }
}