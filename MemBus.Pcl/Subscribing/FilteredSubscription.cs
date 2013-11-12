using System;
using MemBus.Support;

namespace MemBus.Subscribing
{
    /// <summary>
    /// This subscription wraps an inner subscription where the provided filter
    /// is used to decide whether some given message is passed to the inner subscription or not.
    /// </summary>
    public class FilteredSubscription<M> : ISubscription, IDenyShaper
    {
        private readonly Func<M, bool> _filter;
        private readonly ISubscription _subscription;

        public FilteredSubscription(Func<M, bool> filter, ISubscription subscription)
        {
            _filter = filter;
            _subscription = subscription;
        }

        public void Push(object message)
        {
            if (_filter((M) message))
                _subscription.Push(message);
        }

        public bool Handles(Type messageType)
        {
            return _subscription.Handles(messageType);
        }


        public bool Deny
        {
            get { return _subscription.CheckDenyOrAllIsGood(); }
        }
    }
}