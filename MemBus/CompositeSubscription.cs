using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MemBus.Subscribing;
using MemBus.Support;
using System.Linq;

namespace MemBus
{
    internal class CompositeSubscription : ISubscription, IEnumerable<ISubscription>
    {
        private readonly ConcurrentDictionary<int,IDisposableSubscription> subscriptions = new ConcurrentDictionary<int,IDisposableSubscription>();

        public CompositeSubscription() { }

        public CompositeSubscription(IEnumerable<ISubscription> subscriptions)
        {
            AddRange(subscriptions);
        }
        
        public void Push(object message)
        {
            foreach (var s in subscriptions.Values)
                s.Push(message);
        }

        bool ISubscription.Handles(Type messageType)
        {
            return subscriptions.Values.All(s => s.Handles(messageType));
        }

        public event EventHandler Disposed;

        public void Add(ISubscription subscription)
        {
            IDisposableSubscription disposableSub = getDisposableSub(subscription);
            disposableSub.Disposed += onSubscriptionDisposed;
            subscriptions.AddOrUpdate(disposableSub.GetHashCode(), _ => disposableSub, (_,__) => disposableSub);
        }

        private static IDisposableSubscription getDisposableSub(ISubscription subscription)
        {
            return subscription is IDisposableSubscription ? 
                   (IDisposableSubscription)subscription : new DisposableSubscription(subscription);
        }

        private void onSubscriptionDisposed(object sender, EventArgs e)
        {
            IDisposableSubscription value;
            subscriptions.TryRemove(sender.GetHashCode(), out value);
            Disposed.Raise(sender);
        }

        public IEnumerator<ISubscription> GetEnumerator()
        {
            return subscriptions.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddRange(IEnumerable<ISubscription> subscriptions)
        {
            foreach (var s in subscriptions)
                Add(s);
        }
    }
}