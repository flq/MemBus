using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using MemBus.Subscribing;
using MemBus.Support;
using System.Linq;

namespace MemBus
{
	/// <summary>
	/// The composite subscription wraps a number of subscriptions and treats them as one.
	/// This class is not associated with any message type and will accept any kind of class implementing
	/// <see cref="ISubscription"/>. This class also acts as <see cref="ISubscriptionResolver"/>.
	/// </summary>
    public class CompositeSubscription : ISubscription, IEnumerable<ISubscription>, ISubscriptionResolver
    {
        private readonly Dictionary<int,IDisposableSubscription> _subscriptions = new Dictionary<int,IDisposableSubscription>();
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

        public CompositeSubscription() { }

        public CompositeSubscription(IEnumerable<ISubscription> subscriptions)
        {
            AddRange(subscriptions);
        }

		/// <summary>
		/// returns true if no subscriptions are contained
		/// </summary>
        public bool IsEmpty
        {
            get { return _subscriptions.Count == 0; }
        }

		/// <summary>
		/// Pushes the message to the contained subscriptions.
		/// </summary>
        public void Push(object message)
        {
            foreach (var s in Snapshot)
                s.Push(message);
        }

		/// <summary>
		/// This event is raised if any of the contained subscriptions is Disposed.
		/// </summary>
        public event EventHandler Disposed;

        bool ISubscription.Handles(Type messageType)
        {
            return Snapshot.All(s => s.Handles(messageType));
        }

        public IEnumerator<ISubscription> GetEnumerator()
        {
            return Snapshot.GetEnumerator();
        }

        IEnumerable<ISubscription> ISubscriptionResolver.GetSubscriptionsFor(object message)
        {
           return Snapshot.Where(s => s.Handles(message.GetType())).ToArray();
        }

		/// <inheritdoc/>
        public bool Add(ISubscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("subscription", "Attempt to add a Null Reference to Composite subscription.");
            try
            {
                _rwLock.EnterWriteLock();
                JustAdd(subscription);
                return true;
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void AddRange(IEnumerable<ISubscription> subscriptions)
        {
            foreach (var s in subscriptions)
                JustAdd(s);
        }

        private void JustAdd(ISubscription subscription)
        {
            var disposableSub = GetDisposableSub(subscription);
            disposableSub.Disposed += OnSubscriptionDisposed;
            _subscriptions.Add(disposableSub.GetHashCode(), disposableSub);
        }

        private void OnSubscriptionDisposed(object sender, EventArgs e)
        {
            
            try
            {
                _rwLock.EnterWriteLock();
                _subscriptions.Remove(sender.GetHashCode());
                Disposed.Raise(sender);
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        private static IDisposableSubscription GetDisposableSub(ISubscription subscription)
        {
            return subscription is IDisposableSubscription ? 
                (IDisposableSubscription)subscription : new DisposableSubscription(subscription);
        }

        private IReadOnlyCollection<ISubscription> Snapshot
        {
            get
            {
                try
                {
                    _rwLock.EnterReadLock();
                    var disposableSubscriptions = _subscriptions.Values.ToArray();
                    return disposableSubscriptions;
                }
                finally
                {
                    _rwLock.ExitReadLock();
                }
            }
        } 
    }
}