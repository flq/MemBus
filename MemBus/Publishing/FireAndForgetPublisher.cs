using System;
using System.Linq;
using System.Threading.Tasks;

namespace MemBus.Publishing
{
    /// <summary>
    /// Publisher of messages for maximum througput. If any exceptions occur, they will not be honoured in any way.
    /// </summary>
    public class FireAndForgetPublisher : IPublishPipelineMember
    {
        private readonly TaskFactory taskMaker = new TaskFactory();
        
        public void LookAt(PublishToken token)
        {
            token.Subscriptions.Select(s => taskMaker.StartNew(() => s.Push(token.Message))).ToArray();
        }
    }
}