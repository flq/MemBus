using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MemBus.Tests.Help
{
    public class SimpleResolver : ISubscriptionResolver, IEnumerable<ISubscription>
    {
        private readonly List<ISubscription> subscriptions = new List<ISubscription>();

        public IEnumerable<ISubscription> GetSubscriptionsFor(object message)
        {
           if (message == null)
               throw new ArgumentNullException("message");

            return subscriptions.Where(s => message.GetType().Equals(s.Handles));
        }

        public void Add(ISubscription s)
        {
            subscriptions.Add(s);
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