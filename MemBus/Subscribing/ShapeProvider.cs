using System;
using System.Collections.Generic;

namespace MemBus
{
    internal class ShapeProvider
    {
        public IEnumerable<ISubscription> Shape(IEnumerable<ISubscription> subscriptions, object message)
        {
            //TODO: Find shapes by message and apply
            return subscriptions;
        }
    }
}