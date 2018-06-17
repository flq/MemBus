using System.Threading.Tasks;

namespace MemBus.Publishing
{
    /// <summary>
    /// This is the most simple publisher that works like event handlers:
    /// All subscriptions are called in sequence. If any subscription throws an exception the chain is broken.
    /// When used in an async pipeline, all <see cref="IAsyncSubscription"/> s will
    /// also be called in sequence and awaited.
    /// </summary>
    public class SequentialPublisher : IPublishPipelineMember, IAsyncPublishPipelineMember
    {
        void IPublishPipelineMember.LookAt(PublishToken token)
        {
            foreach (var s in token.Subscriptions)
                s.Push(token.Message);
        }

        async Task IAsyncPublishPipelineMember.LookAtAsync(AsyncPublishToken token)
        {
            foreach (var s in token.Subscriptions)
                await s.PushAsync(token.Message);
        }
    }
}