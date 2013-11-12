namespace MemBus.Subscribing
{
    /// <summary>
    /// This may be implemented by implementors of <see cref="ISubscription"/>
    /// If implemented, the property returns the instance which is subscribed.
    /// Note that this may be null if the target is a static method.
    /// </summary>
    public interface IKnowsSubscribedInstance
    {
        /// <summary>
        /// The instance that "belongs" to a subscription or null, if the subscription is based on a static method
        /// </summary>
        object Instance { get; }
    }
}