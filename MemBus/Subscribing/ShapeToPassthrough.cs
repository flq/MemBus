
namespace MemBus.Subscribing
{
    /// <summary>
    /// Application of the NullObject-Pattern to subscription shapers.
    /// </summary>
    public class ShapeToPassthrough : ISubscriptionShaper
    {
        public ISubscription EnhanceSubscription(ISubscription subscription)
        {
            return subscription;
        }
    }
}