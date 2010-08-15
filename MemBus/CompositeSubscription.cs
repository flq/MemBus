using System;
using System.Collections;
using System.Collections.Generic;
using MemBus.Subscribing;
using MemBus.Support;
using System.Linq;

namespace MemBus
{
    internal class CompositeSubscription : ISubscription, IEnumerable<ISubscription>
    {
        private Type handledType;
        //TODO: This should be some thread-safe construct.
        private readonly List<IDisposableSubscription> subscriptions = new List<IDisposableSubscription>();


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

            var disposableSub = subscription is IDisposableSubscription ? 
                (IDisposableSubscription)subscription : new DisposableSubscription(subscription);
            disposableSub.Disposed += onSubscriptionDisposed;
            subscriptions.Add(disposableSub);
        }

        private void onSubscriptionDisposed(object sender, EventArgs e)
        {
            subscriptions.Remove((IDisposableSubscription)sender);
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