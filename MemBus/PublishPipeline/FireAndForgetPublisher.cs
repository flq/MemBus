using System;
using System.Threading;

namespace MemBus
{
    public class FireAndForgetPublisher : IPublishPipelineMember
    {
        public void LookAt(PublishToken token)
        {
            foreach (var s in token.Subscriptions)
                ThreadPool.QueueUserWorkItem(obj => ((ISubscription) obj).Push(token.Message), s);
        }
    }
}