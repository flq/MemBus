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

        private readonly HashSet<Type> seenMessageTypes = new HashSet<Type>();
        private volatile bool newSubscriptions = false;

        public CachingResolver()
        {
            cachedSubscriptions.TryAdd(typeof(ImpossibleMessage), new CompositeSubscription());
        }

        public IEnumerable<ISubscription> GetSubscriptionsFor(object message)
        {
            if (seenMessageTypes.Contains(message.GetType()) && !newSubscriptions)
                return cachedSubscriptions[message.GetType()].AsEnumerable();
            var lookAtThis =
                cachedSubscriptions[typeof(ImpossibleMessage)].Where(s => s.Handles(message.GetType())).ToArray();
            cachedSubscriptions.AddOrUpdate(message.GetType(),
                                            _ => new CompositeSubscription(lookAtThis),
                                            (_, composite) =>
                                            {
                                                composite.AddRange(lookAtThis);
                                                return composite;
                                            });
            seenMessageTypes.Add(message.GetType());
            //newSubscriptions = false;
            return cachedSubscriptions[message.GetType()].AsEnumerable();
        }

        public bool Add(ISubscription subscription)
        {
            cachedSubscriptions[typeof(ImpossibleMessage)].Add(subscription);
            newSubscriptions = true;
            return true;
        }
    }

    //internal class CachingResolver : ISubscriptionResolver
    //{
    //    private static class ImpossibleMessage { }
    //    private readonly ConcurrentDictionary<Type, CompositeSubscription> cachedSubscriptions = new ConcurrentDictionary<Type, CompositeSubscription>();

    //    private readonly HashSet<Type> seenMessageTypes = new HashSet<Type>();

    //    public CachingResolver()
    //    {
    //        cachedSubscriptions.TryAdd(typeof(ImpossibleMessage), new CompositeSubscription());
    //    }

    //    public IEnumerable<ISubscription> GetSubscriptionsFor(object message)
    //    {
    //        if (seenMessageTypes.Contains(message.GetType()) && NoNewSubscriptions)
    //            return cachedSubscriptions[message.GetType()].AsEnumerable();
    //        var newSubscriptions =
    //            cachedSubscriptions[typeof(ImpossibleMessage)].Where(s => s.Handles(message.GetType())).ToArray();
    //        cachedSubscriptions.AddOrUpdate(message.GetType(),
    //                                        _ => new CompositeSubscription(newSubscriptions),
    //                                        (_, composite) =>
    //                                        {
    //                                            composite.AddRange(newSubscriptions);
    //                                            return composite;
    //                                        });

    //        cachedSubscriptions.AddOrUpdate(typeof(ImpossibleMessage), new CompositeSubscription(),
    //                                        (_, composite) =>
    //                                        {
    //                                            composite.RemoveRange(newSubscriptions);
    //                                            return composite;
    //                                        });

    //        seenMessageTypes.Add(message.GetType());
    //        return cachedSubscriptions[message.GetType()].AsEnumerable();
    //    }

    //    private bool NoNewSubscriptions
    //    {
    //        get { return cachedSubscriptions[typeof(ImpossibleMessage)].IsEmpty; }
    //    }

    //    public bool Add(ISubscription subscription)
    //    {
    //        var knownType = seenMessageTypes.FirstOrDefault(subscription.Handles);
    //        cachedSubscriptions[knownType ?? typeof(ImpossibleMessage)].Add(subscription);
    //        return true;
    //    }
    //}
}