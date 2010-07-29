using System;
using Rf.Common;

namespace MemBus.Subscribing
{
    public class SubscriptionCustomizer<M> : ISubscriptionCustomizer<M>
    {
        public ISubscriptionCustomizer<M> SetFilter(Func<M, bool> filter)
        {
            throw new NotImplementedException();
        }

        public ISubscriptionShape AsShape()
        {
            throw new NotImplementedException();
        }

        public ISubscription ConstructSubscription<M>(Action<M> parameters)
        {
            throw new NotImplementedException();
        }
    }
}