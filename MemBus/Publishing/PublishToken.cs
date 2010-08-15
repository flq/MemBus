using System.Collections.Generic;

namespace MemBus
{
    public class PublishToken
    {
        public object Message { get; private set; }
        public IEnumerable<ISubscription> Subscriptions { get; private set; }

        public PublishToken(object message, IEnumerable<ISubscription> subscriptions)
        {
            Message = message;
            Subscriptions = subscriptions;
        }
    }
}