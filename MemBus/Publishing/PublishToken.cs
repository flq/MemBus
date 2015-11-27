using System.Collections.Generic;

namespace MemBus.Publishing
{
    public class PublishToken
    {
        public object Message { get; private set; }
        public IEnumerable<ISubscription> Subscriptions { get; private set; }

        /// <summary>
        /// When set to true, the subsequent members in the publish pipeline will not be called anymore
        /// </summary>
        public bool Cancel { get; set; }

        public PublishToken(object message, IEnumerable<ISubscription> subscriptions)
        {
            Message = message;
            Subscriptions = subscriptions;
        }
    }
}