using MemBus;
using Membus.WpfTwitterClient.Frame;
using Membus.WpfTwitterClient.Frame.Twitter;
using Membus.WpfTwitterClient.Messages;

namespace Membus.WpfTwitterClient.Handler
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