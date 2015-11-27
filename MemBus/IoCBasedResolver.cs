using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using MemBus.Subscribing;

namespace MemBus
{
    internal class IoCBasedResolver : ISubscriptionResolver
    {
        private readonly IocAdapter _adapter;
        private readonly Type _handlerType;
        private readonly Func<Type, Type> _messageTypeResolver;
        private readonly Dictionary<Type, Type> _typeCache = new Dictionary<Type, Type>();
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

        public IoCBasedResolver(IocAdapter adapter, Type handlerType, Func<Type, Type> messageTypeResolver)
        {
            _adapter = adapter;
            _handlerType = handlerType;
            _messageTypeResolver = messageTypeResolver;
        }

        public IEnumerable<ISubscription> GetSubscriptionsFor(object message)
        {
            var handlesType = ConstructHandlesType(message.GetType());
            var mi = handlesType.GetRuntimeMethods().First();
            return _adapter.GetAllInstances(handlesType)
                .Select(mi.ConstructSubscription);
        }

        private Type ConstructHandlesType(Type messageType)
        {
            try
            {
                _rwLock.EnterUpgradeableReadLock();
                if (!_typeCache.ContainsKey(messageType))
                {
                    try
                    {
                        _rwLock.EnterWriteLock();
                        _typeCache[messageType] = _handlerType.MakeGenericType(_messageTypeResolver(messageType));
                    }
                    finally
                    {
                        _rwLock.ExitWriteLock();
                    }
                }
                return _typeCache[messageType];
            }
            finally
            {
                _rwLock.ExitUpgradeableReadLock();
            }
        }

        public bool Add(ISubscription subscription)
        {
            // We just resolve here, no adding.
            return false;
        }
    }
}