using System;
using MemBus;
using Membus.WpfTwitterClient.Frame;
using Membus.WpfTwitterClient.Frame.Twitter;
using Membus.WpfTwitterClient.Frame.UI;
using Membus.WpfTwitterClient.Timeline;

namespace Membus.WpfTwitterClient.GatherAccessToken
{
    public class GetAndStoreAccessTokenHandler : Handles<RequestToGetAccessToken>
    {
        private readonly IBus bus;
        private readonly ITwitterSession session;
        private readonly IUserSettings settings;

        public GetAndStoreAccessTokenHandler(IBus bus, ITwitterSession session, IUserSettings settings)
        {
            this.bus = bus;
            this.session = session;
            this.settings = settings;
        }

        protected override void push(RequestToGetAccessToken message)
        {
            session.GetAccessToken(message.Pin, token =>
                                                    {
                                                        settings.StoreAccessToken(token);
                                                        bus.Publish(new RequestToActivateMainScreen(typeof(TimelinesViewModel)));
                                                    });
        }
    }
}