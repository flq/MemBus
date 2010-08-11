namespace MemBus.Subscribing
{
    /// <summary>
    /// Use this on a <see cref="ISubscription" /> implementation to state that 
    /// this implementation should be skipped when applying a convention driven <see cref="SubscriptionMatroschka"/>
    /// </summary>
    public interface IDenyShaper
    {
        bool Deny { get; }
    }
}