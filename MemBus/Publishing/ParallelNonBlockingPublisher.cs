using System;
using System.Linq;
using System.Threading.Tasks;
using MemBus.Messages;
using MemBus.Support;

namespace MemBus.Publishing
{
    /// <summary>
    /// Publisher of messages that publishes in parallel and does not wait for all handlers to return.
    /// When all subscriptions have done their work, any exceptions will be collected and put on the bus
    /// as <see cref="ExceptionOccurred"/> messages.
    /// </summary>
    public class ParallelNonBlockingPublisher : IPublishPipelineMember, IRequireBus
    {
        private readonly TaskFactory taskMaker = new TaskFactory();
        private IBus _bus;

        
        public void LookAt(PublishToken token)
        {
            var tasks = token.Subscriptions.Select(s => taskMaker.StartNew(() => s.Push(token.Message))).ToArray();
            if (tasks.Length == 0)
                return;
            taskMaker.ContinueWhenAll(tasks,
                                      ts =>
                                          {
                                              //TODO: How to catch this exception? Seems to go to Nirvana...
                                              if (token.Message is ExceptionOccurred && ts.Any(t=>t.IsFaulted))
                                                  throw new MemBusException("Possible infinite messaging cycle since handling ExceptionOccurred has produced unhandled exceptions!");
                                              ts.PublishExceptionMessages(_bus);
                                          });
        }

        void IRequireBus.AddBus(IBus bus)
        {
            if (bus == null)
              throw new InvalidOperationException("This publisher requires a bus for exception propagation");
            _bus = bus; 
        }
    }
}