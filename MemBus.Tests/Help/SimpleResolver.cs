using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MemBus.Support;

namespace MemBus.Tests.Help
{
    public class SimpleResolver : ISubscriptionResolver, IRequireServices, IEnumerable<ISubscription>
    {
        private readonly List<ISubscription> _subscriptions = new List<ISubscription>();

        public IServices Services { get; private set; }

        public void AddServices(IServices svc) 
        {
            Services = svc;
        }
        public IEnumerable<ISubscription> GetSubscriptionsFor(object message)
        {
           if (message == null)
               throw new ArgumentNullException("message");

            return _subscriptions.Where(s => s.Handles(message.GetType()));
        }

        public bool Add(ISubscription s)
        {
            _subscriptions.Add(s);
            return true;
        }

        public IEnumerator<ISubscription> GetEnumerator()
        {
            return _subscriptions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}