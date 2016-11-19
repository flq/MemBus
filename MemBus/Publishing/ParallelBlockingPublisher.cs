using System.Threading.Tasks;
using System.Linq;
using MemBus.Support;
using System;

namespace MemBus.Publishing
{
    /// <summary>
    /// Calls subscribers in parallel but blocks until all subscriptions return.
    /// </summary>
    public class ParallelBlockingPublisher : IPublishPipelineMember, IRequireBus
    {
        private readonly TaskFactory _taskMaker = new TaskFactory();
        private IBus _bus;

        public void LookAt(PublishToken token)
        {
            var tasks = token.Subscriptions.Select(s => _taskMaker.StartNew(() => s.Push(token.Message))).ToArray();
            Task.WaitAll(tasks);
            tasks.PublishExceptionMessages(_bus);
        }

        void IRequireBus.AddBus(IBus bus)
        {
            _bus = bus;
        }
    }
}