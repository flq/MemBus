using System.Threading.Tasks;
using System.Linq;
using MemBus.Messages;
using MemBus.Support;

namespace MemBus
{
    /// <summary>
    /// Calls subscribers in parallel but blocks until all subscriptions return.
    /// </summary>
    public class ParallelPublisher : IPublishPipelineMember
    {
        private readonly TaskFactory taskMaker = new TaskFactory();
        private IBus bus;

        public IBus Bus
        {
            set { bus = value; }
        }

        public void LookAt(PublishToken token)
        {
            var tasks = (token.Subscriptions.Select(s => taskMaker.StartNew(() => s.Push(token.Message)))).ToArray();
            Task.WaitAll(tasks);

            tasks
                .Where(t => t.Exception != null)
                .Select(t => t.Exception)
                .Each(e => bus.Publish(new ExceptionOccurred(e)));
        }
    }
}