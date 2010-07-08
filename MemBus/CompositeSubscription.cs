using System;
using System.Collections;
using System.Collections.Generic;

namespace MemBus
{
    public class CompositeSubscription : ISubscription, IEnumerable<ISubscription>
    {
        private Type handledType;
        private readonly List<ISubscription> subscriptions = new List<ISubscription>();


        public void Push(object message)
        {
            foreach (var s in subscriptions)
                s.Push(message);
        }

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
            subscriptions.Add(subscription);
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