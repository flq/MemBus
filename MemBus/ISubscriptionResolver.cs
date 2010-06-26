using System.Collections.Generic;

namespace MemBus
{
    /// <summary>
    /// Describes a component that for a given message returns subscriptions that want to handle it
    /// </summary>
    public interface ISubscriptionResolver
    {
        IEnumerable<ISubscription> GetRelevantSubscriptions(object message);
    }
}