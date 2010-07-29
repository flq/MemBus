using System;
using Rf.Common;

namespace MemBus.Subscribing
{
    public class StandardShape : ISubscriptionShape
    {
        public ISubscription ConstructSubscription<T>(Action<T> action)
        {
            return new DisposableMethodSubscription<T>(action);
        }
    }
}