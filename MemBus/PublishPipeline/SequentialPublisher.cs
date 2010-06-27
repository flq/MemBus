using System;

namespace MemBus
{
    public class SequentialPublisher : IPublishPipelineMember
    {
        public void LookAt(PublishToken token)
        {
            foreach (var s in token.Subscriptions)
                s.Push(token.Message);
        }
    }
}