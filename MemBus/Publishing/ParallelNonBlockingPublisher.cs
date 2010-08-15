using System;
using System.Linq;
using System.Threading.Tasks;
using MemBus.Messages;
using MemBus.Support;

namespace MemBus.Publishing
{
    /// <summary>
    /// Publisher of messages that publishes in parallel and does not wait for all handlers to return.
    /// </summary>
    public class ParallelNonBlockingPublisher : IPublishPipelineMember
    {
        private readonly TaskFactory taskMaker = new TaskFactory();
        private IBus bus;

        public IBus Bus
        {
            set
            {
                if (value == null)
                    throw new InvalidOperationException("This publisher requires a bus for exception propagation");
                bus = value;
            }
        }

        public void LookAt(PublishToken token)
        {
            var tasks = token.Subscriptions.Select(s => taskMaker.StartNew(() => s.Push(token.Message))).ToArray();
            taskMaker.ContinueWhenAll(tasks,
                                      ts =>
                                          {
                                              //TODO: How to catch this exception? Seems to go to Nirvana...
                                              if (ts.Any(t=>t.IsFaulted) && token.Message is ExceptionOccurred)
                                                  throw new MemBusException("Possible infinite messaging cycle since handling ExceptionOccurred has produced unhandled exceptions!");
                                              ts.ConvertToExceptionMessages().Each(e => bus.Publish(e));
                                          });
        }
    }
}