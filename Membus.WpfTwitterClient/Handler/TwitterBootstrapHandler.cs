using MemBus;
using Membus.Tests.WpfClient.Frame;
using Membus.Tests.WpfClient.Messages;

namespace Membus.Tests.WpfClient.Handler
{
    public class TwitterBootstrapHandler : Handles<Bootstrap>
    {
        private readonly TwitterKeys keys;

        public TwitterBootstrapHandler(TwitterKeys keys)
        {
            this.keys = keys;
        }

        protected override void push(Bootstrap message)
        {
            
        }
    }
}