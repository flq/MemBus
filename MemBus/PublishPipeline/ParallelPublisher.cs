using System.Threading.Tasks;

namespace MemBus
{
    /// <summary>
    /// Calls subscribers in parallel but blocks until all subscriptions return.
    /// </summary>
    public class ParallelPublisher : IPublishPipelineMember
    {
        public void LookAt(PublishToken token)
        {
            Parallel.ForEach(token.Subscriptions, s => s.Push(token.Message));
        }
    }
}