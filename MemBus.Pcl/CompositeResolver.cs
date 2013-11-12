using System;
using System.Collections.Generic;
using System.Linq;

namespace MemBus
{
    internal class CompositeResolver : ISubscriptionResolver, IDisposable
    {
        readonly List<ISubscriptionResolver> _resolvers = new List<ISubscriptionResolver>();

        public CompositeResolver(params ISubscriptionResolver[] resolvers)
        {
            _resolvers.AddRange(resolvers);
        }

        public IEnumerable<ISubscription> GetSubscriptionsFor(object message)
        {
            return _resolvers.SelectMany(r => r.GetSubscriptionsFor(message));
        }

        public void Add(ISubscriptionResolver resolver)
        {
            _resolvers.Add(resolver);
        }

        public bool Add(ISubscription subscription)
        {
            var wasAdded = false;
            foreach (var resolver in _resolvers)
            {
                wasAdded = resolver.Add(subscription);
                if (wasAdded) break;
            }
            return wasAdded;
        }

        public void Dispose()
        {
            _resolvers.Clear();
        }
    }
}