using System.Linq;
using System.Threading.Tasks;

namespace MemBus.Publishing
{
    /// <summary>
    /// This publish pipeline member
    /// awaits all Subscriptions before yielding back to the caller
    /// </summary>
    public class WhenAllTaskPublisher : IAsyncPublishPipelineMember
    {
        public Task LookAtAsync(AsyncPublishToken token)
        {
            return Task.WhenAll(token.Subscriptions.Select(sub => sub.PushAsync(token.Message)));
        }
    }
}