using System.Collections.Generic;
using System.Linq;

namespace MemBus
{
    internal class StandardResolver : ISubscriptionResolver
    {
        private readonly CompositeSubscription _cS;

        public StandardResolver()
        {
            _cS = new CompositeSubscription();
        }

        public IEnumerable<ISubscription> GetSubscriptionsFor(object message)
        {
            var lookAtThis = _cS.Where(s => s.Handles(message.GetType())).ToArray();
            return lookAtThis;
        }

        public bool Add(ISubscription subscription)
        {
            _cS.Add(subscription);
            return true;
        }
    }
}