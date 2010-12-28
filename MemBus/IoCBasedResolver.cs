using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MemBus
{
    internal class IoCBasedResolver : ISubscriptionResolver
    {

        private readonly IocAdapter adapter;

        private readonly ConcurrentDictionary<Type, Type> typeCache = new ConcurrentDictionary<Type, Type>();

        public IoCBasedResolver(IocAdapter adapter)
        {
            this.adapter = adapter;
        }


        public IEnumerable<ISubscription> GetSubscriptionsFor(object message)
        {
            var handlesType = constructHandlesType(message.GetType());
            return adapter.GetAllInstances(handlesType).Select(svc => (ISubscription) svc);
        }

        private Type constructHandlesType(Type messageType)
        {
            return typeCache.GetOrAdd(messageType, msgT => typeof (IHandles<>).MakeGenericType(messageType));
        }

        public bool Add(ISubscription subscription)
        {
            // We just resolve here, no adding.
            return false;
        }
    }
}