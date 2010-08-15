using System;

namespace MemBus
{
    /// <summary>
    /// This is the most simple publisher that works like event handlers: All subscriptions are called in sequence.
    /// If any subscription throws an exception the chain is broken.
    /// </summary>
    public class SequentialPublisher : IPublishPipelineMember
    {
        public void LookAt(PublishToken token)
        {
            foreach (var s in token.Subscriptions)
                s.Push(token.Message);
        }
    }
}