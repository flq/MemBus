using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MemBus.Support;

namespace MemBus.Tests.Help
{
    public class SimpleResolver : ISubscriptionResolver, IEnumerable<ISubscription>
    {
        private readonly List<ISubscription> subscriptions = new List<ISubscription>();

        public IServices Services { get; set; }

        public IEnumerable<ISubscription> GetSubscriptionsFor(object message)
        {
           if (message == null)
               throw new ArgumentNullException("message");

            return subscriptions.Where(s => s.Handles(message.GetType()));
        }

        public bool Add(ISubscription s)
        {
            subscriptions.Add(s);
            return true;
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