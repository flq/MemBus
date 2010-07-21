using System;
using Rf.Common;

namespace MemBus.Subscribing
{
    public class StandardShape : ISubscriptionShape
    {
        public ISubscription ConstructSubscription<T>(IServices parameters)
        {
            return new DisposableMethodSubscription<T>(parameters.Get<Action<T>>());
        }
    }
}