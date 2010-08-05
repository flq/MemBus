using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MemBus.Support;

namespace MemBus
{
    public class ParallelNonBlockingPublisher : IPublishPipelineMember
    {
        private readonly TaskFactory taskMaker = new TaskFactory();
        private IBus bus;

        public IBus Bus
        {
            set { bus = value; }
        }
        public void LookAt(PublishToken token)
        {
            var tasks = token.Subscriptions.Select(s => taskMaker.StartNew(() => s.Push(token.Message))).ToArray();
            taskMaker.ContinueWhenAll(tasks, ts => ts.ConvertToExceptionMessages().Each(e => bus.Publish(e)));
        }
    }
}