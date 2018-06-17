using System.Collections.Generic;

namespace MemBus.Publishing
{
    public class AsyncPublishToken
    {
        public object Message { get; }
        public IEnumerable<IAsyncSubscription> Subscriptions { get; }

        /// <summary>
        /// When set to true, the subsequent members in the publish pipeline will not be called anymore
        /// </summary>
        public bool Cancel { get; set; }

        public AsyncPublishToken(object message, IEnumerable<IAsyncSubscription> subscriptions)
        {
            Message = message;
            Subscriptions = subscriptions;
        }
    }
}