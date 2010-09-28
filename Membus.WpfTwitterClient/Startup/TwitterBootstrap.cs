using System;
using MemBus;
using Membus.WpfTwitterClient.Frame;
using Membus.WpfTwitterClient.Frame.Twitter;
using Membus.WpfTwitterClient.GatherAccessToken;
using Membus.WpfTwitterClient.Properties;

namespace Membus.WpfTwitterClient.Startup
{
    public class TwitterBootstrap : Handles<Bootstrap>
    {
        private readonly TwitterKeys keys;
        private readonly IUserSettings settings;
        private readonly IBus bus;

        public TwitterBootstrap(TwitterKeys keys, IUserSettings settings, IBus bus)
        {
            this.keys = keys;
            this.settings = settings;
            this.bus = bus;
        }

        protected override void push(Bootstrap message)
        {
            if (string.IsNullOrEmpty(settings.AccessToken))
                bus.Publish(new RequestToGatherAccessToken(keys));
        }
    }
}