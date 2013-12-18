using System.Threading.Tasks;

namespace MemBus.Publishing
{
    /// <summary>
    /// This is the most simple publisher that works like event handlers: All subscriptions are called in sequence.
    /// If any subscription throws an exception the chain is broken.
    /// </summary>
    public class SequentialPublisher : IPublishPipelineMember, IAsyncPublishPipelineMember
    {
        public void LookAt(PublishToken token)
        {
            foreach (var s in token.Subscriptions)
                s.Push(token.Message);
        }

        public async Task LookAtAsync(PublishToken token)
        {
            LookAt(token);
        }
    }
}