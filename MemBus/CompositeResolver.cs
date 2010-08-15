using System;
using System.Collections.Generic;
using System.Linq;

namespace MemBus
{
    internal class CompositeResolver : ISubscriptionResolver
    {
        readonly List<ISubscriptionResolver> resolvers = new List<ISubscriptionResolver>();

        public CompositeResolver(params ISubscriptionResolver[] resolvers)
        {
            this.resolvers.AddRange(resolvers);
        }

        public IEnumerable<ISubscription> GetSubscriptionsFor(object message)
        {
            
            return resolvers.Select(r => r.GetSubscriptionsFor(message)).SelectMany(s => s);
        }

        public void Add(ISubscriptionResolver resolver)
        {
            resolvers.Add(resolver);
        }

        public bool Add(ISubscription subscription)
        {
            var wasAdded = false;
            foreach (var resolver in resolvers)
            {
                wasAdded = resolver.Add(subscription);
                if (wasAdded) break;
            }
            return wasAdded;
        }
    }
}