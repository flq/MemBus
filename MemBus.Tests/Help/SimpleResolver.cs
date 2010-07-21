using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rf.Common;

namespace MemBus.Tests.Help
{
    public class SimpleResolver : ISubscriptionResolver, IEnumerable<ISubscription>
    {
        private readonly List<ISubscription> subscriptions = new List<ISubscription>();
        private IServices services;

        public IServices Services
        {
            get { return services; }
        }

        public IEnumerable<ISubscription> GetSubscriptionsFor(object message)
        {
           if (message == null)
               throw new ArgumentNullException("message");

            return subscriptions.Where(s => message.GetType().Equals(s.Handles));
        }

        public bool Add(ISubscription s)
        {
            subscriptions.Add(s);
            return true;
        }

        public void AcceptServices(IServices services)
        {
            this.services = services;
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