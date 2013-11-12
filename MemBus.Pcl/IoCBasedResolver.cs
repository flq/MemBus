using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MemBus.Subscribing;

namespace MemBus
{
    internal class IoCBasedResolver : ISubscriptionResolver
    {
        private readonly IocAdapter adapter;
        private readonly Type _handlerType;
        private readonly Func<Type, Type> _messageTypeResolver;

        private readonly ConcurrentDictionary<Type, Type> typeCache = new ConcurrentDictionary<Type, Type>();

        public IoCBasedResolver(IocAdapter adapter, Type handlerType, Func<Type, Type> messageTypeResolver)
        {
            this.adapter = adapter;
            _handlerType = handlerType;
            _messageTypeResolver = messageTypeResolver;
        }

        public IEnumerable<ISubscription> GetSubscriptionsFor(object message)
        {
            var handlesType = constructHandlesType(message.GetType());
            var mi = handlesType.GetRuntimeMethods().First();
            return adapter.GetAllInstances(handlesType)
                .Select(svc => mi.ConstructSubscription(svc));
        }

        private Type constructHandlesType(Type messageType)
        {
            return typeCache.GetOrAdd(messageType, msgT => _handlerType.MakeGenericType(_messageTypeResolver(msgT)));
        }

        public bool Add(ISubscription subscription)
        {
            // We just resolve here, no adding.
            return false;
        }
    }
}