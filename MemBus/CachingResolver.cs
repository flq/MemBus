using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MemBus
{
    internal class CachingResolver : ISubscriptionResolver
    {
        private static class ImpossibleMessage { }
        private readonly ConcurrentDictionary<Type, CompositeSubscription> cachedSubscriptions = new ConcurrentDictionary<Type, CompositeSubscription>();

        public CachingResolver()
        {
            cachedSubscriptions.TryAdd(typeof(ImpossibleMessage), new CompositeSubscription());
        }

        public IEnumerable<ISubscription> GetSubscriptionsFor(object message)
        {
            var lookAtThis =
                cachedSubscriptions[typeof(ImpossibleMessage)].Where(s => s.Handles(message.GetType())).ToArray();
            return lookAtThis;
        }

        public bool Add(ISubscription subscription)
        {
            cachedSubscriptions[typeof(ImpossibleMessage)].Add(subscription);
            return true;
        }
    }
}