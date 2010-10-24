using MemBus;
using Membus.WpfTwitterClient.Frame;
using Membus.WpfTwitterClient.Frame.UI;
using Membus.WpfTwitterClient.GatherAccessToken;

namespace Membus.WpfTwitterClient.Startup
{
    public class TwitterBootstrap : Handles<RequestToStartup>
    {
        private readonly IUserSettings settings;
        private readonly IBus bus;

        public TwitterBootstrap(IUserSettings settings, IBus bus)
        {
            this.settings = settings;
            this.bus = bus;
        }

        protected override void push(RequestToStartup message)
        {
            if (string.IsNullOrEmpty(settings.AccessToken))
                bus.Publish(new RequestToActivateMainScreen(typeof(GetAccessTokenViewModel)));
        }
    }
}