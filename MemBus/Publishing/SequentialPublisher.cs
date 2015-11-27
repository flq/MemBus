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

        #pragma warning disable 1998 //This is just a route through method, so it's OK if this part does run synchronously
        public async Task LookAtAsync(PublishToken token)
        {
            await Task.Run(() => LookAt(token));
        }
    }
}