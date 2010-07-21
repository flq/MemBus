using System;
using Rf.Common;

namespace MemBus
{
    public interface ISubscriptionShape
    {
        ISubscription ConstructSubscription<T>(IServices parameters);
    }
}