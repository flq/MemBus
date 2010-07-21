using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MemBus.Support;
using System.Linq;

namespace MemBus
{
    public class CompositeSubscription : ISubscription, IEnumerable<ISubscription>
    {
        private Type handledType;
        //TODO: This should be some thread-safe construct.
        private readonly List<ISubscription> subscriptions = new List<ISubscription>();


        public void Push(object message)
        {
            foreach (var s in subscriptions)
                s.Push(message);
        }

        public IDisposable GetDisposer()
        {
            return new CompositeDisposer(subscriptions.Select(s => s.GetDisposer()));
        }

        public event EventHandler Disposed;

        public Type Handles
        {
            get
            {
                if (handledType == null)
                    throw new InvalidOperationException("Empty container cannot answer handled type faithfully.");
                return handledType;
            }
        }

        public void Add(ISubscription subscription)
        {
            if (handledType != null && !handledType.Equals(subscription.Handles))
                throw new InvalidOperationException(string.Format("Subscription does not handle {0}", handledType.Name));
            handledType = subscription.Handles;
            subscription.Disposed += onSubscriptionDisposed;
            subscriptions.Add(subscription);
        }

        private void onSubscriptionDisposed(object sender, EventArgs e)
        {
            subscriptions.Remove((ISubscription)sender);
            Disposed.Raise(sender);
        }

        public IEnumerator<ISubscription> GetEnumerator()
        {
            return subscriptions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}