using System.Threading.Tasks;
using System.Linq;
using MemBus.Support;

namespace MemBus.Publishing
{
    /// <summary>
    /// Calls subscribers in parallel but blocks until all subscriptions return.
    /// </summary>
    public class ParallelBlockingPublisher : IPublishPipelineMember
    {
        private readonly TaskFactory _taskMaker = new TaskFactory();
        private IBus bus;

        public IBus Bus
        {
            set { bus = value; }
        }

        public void LookAt(PublishToken token)
        {
            var tasks = token.Subscriptions.Select(s => _taskMaker.StartNew(() => s.Push(token.Message))).ToArray();
            Task.WaitAll(tasks);
            tasks.PublishExceptionMessages(bus);
        }
    }
}