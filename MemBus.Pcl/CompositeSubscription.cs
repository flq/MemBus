using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MemBus.Subscribing;
using MemBus.Support;
using System.Linq;

namespace MemBus
{
    internal class CompositeSubscription : ISubscription, IEnumerable<ISubscription>, ISubscriptionResolver
    {
        // this would have to be replace to target WP8 - http://stackoverflow.com/questions/18367839/alternative-to-concurrentdictionary-for-portable-class-library
        private readonly ConcurrentDictionary<int,IDisposableSubscription> _subscriptions = new ConcurrentDictionary<int,IDisposableSubscription>();

        public CompositeSubscription() { }

        public CompositeSubscription(IEnumerable<ISubscription> subscriptions)
        {
            AddRange(subscriptions);
        }

        public bool IsEmpty
        {
            get { return _subscriptions.IsEmpty; }
        }

        public void Push(object message)
        {
            foreach (var s in _subscriptions.Values)
                s.Push(message);
        }

        bool ISubscription.Handles(Type messageType)
        {
            return _subscriptions.Values.All(s => s.Handles(messageType));
        }

        public event EventHandler Disposed;

        public IEnumerator<ISubscription> GetEnumerator()
        {
            return _subscriptions.Values.GetEnumerator();
        }

        private static IDisposableSubscription GetDisposableSub(ISubscription subscription)
        {
            return subscription is IDisposableSubscription ? 
                   (IDisposableSubscription)subscription : new DisposableSubscription(subscription);
        }

        private void OnSubscriptionDisposed(object sender, EventArgs e)
        {
            IDisposableSubscription value;
            _subscriptions.TryRemove(sender.GetHashCode(), out value);
            Disposed.Raise(sender);
        }

        IEnumerable<ISubscription> ISubscriptionResolver.GetSubscriptionsFor(object message)
        {
           return _subscriptions.Values.Where(s => s.Handles(message.GetType())).ToArray();
        }

        public bool Add(ISubscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription", "Attempt to add a Null Reference to Composite subscription.");
            IDisposableSubscription disposableSub = GetDisposableSub(subscription);
            disposableSub.Disposed += OnSubscriptionDisposed;
            _subscriptions.AddOrUpdate(disposableSub.GetHashCode(), _ => disposableSub, (_, __) => disposableSub);
            return true;
        }

       

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void AddRange(IEnumerable<ISubscription> subscriptions)
        {
            foreach (var s in subscriptions)
                Add(s);
        }

    }
}