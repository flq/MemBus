using MemBus;
using Membus.WpfTwitterClient.Frame;
using Membus.WpfTwitterClient.Frame.UI;
using Membus.WpfTwitterClient.GatherAccessToken;
using Membus.WpfTwitterClient.Timeline;

namespace Membus.WpfTwitterClient.Startup
{
    public class StartupHandler : Handles<RequestToStartup>
    {
        private readonly IUserSettings settings;
        private readonly IBus bus;

        public StartupHandler(IUserSettings settings, IBus bus)
        {
            this.settings = settings;
            this.bus = bus;
        }

        protected override void push(RequestToStartup message)
        {
            var screenToActivate = settings.IsAccessTokenAvailable
                                       ? typeof (TimelinesViewModel)
                                       : typeof (GetAccessTokenViewModel);
            bus.Publish(new RequestToActivateMainScreen(screenToActivate));
        }
    }
}