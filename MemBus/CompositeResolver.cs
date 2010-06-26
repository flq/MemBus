using System;
using System.Collections.Generic;
using System.Linq;

namespace MemBus
{
    public class CompositeResolver : ISubscriptionResolver
    {
        readonly List<ISubscriptionResolver> resolvers = new List<ISubscriptionResolver>();

        public CompositeResolver(params ISubscriptionResolver[] resolvers)
        {
            this.resolvers.AddRange(resolvers);
        }

        public IEnumerable<ISubscription> GetRelevantSubscriptions(object message)
        {
            return resolvers.Select(r => r.GetRelevantSubscriptions(message)).SelectMany(s => s);
        }
    }
}