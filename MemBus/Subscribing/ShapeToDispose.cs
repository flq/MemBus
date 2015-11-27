namespace MemBus.Subscribing
{
    /// <summary>
    /// Use this shape to make your subscription disposable. MemBus will usually apply this shaper
    /// as last one in a pipeline by default.
    /// </summary>
    public class ShapeToDispose : ISubscriptionShaper
    {
        public ISubscription EnhanceSubscription(ISubscription subscription)
        {
            return subscription is IDisposableSubscription ? subscription : new DisposableSubscription(subscription);
        }
    }
}