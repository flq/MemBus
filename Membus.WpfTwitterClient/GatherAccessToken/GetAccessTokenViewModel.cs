using System;
using Caliburn.Micro;
using MemBus;
using Membus.WpfTwitterClient.Frame;
using Membus.WpfTwitterClient.Frame.Twitter;
using Membus.WpfTwitterClient.Frame.UI;

namespace Membus.WpfTwitterClient.GatherAccessToken
{
    public class GetAccessTokenViewModel : Screen
    {
        private readonly ITwitterSession session;
        private readonly IBus bus;

        public GetAccessTokenViewModel(ITwitterSession session, IBus bus)
        {
            DisplayName = "Authorize Membus OnTweet to interact with Twitter on your behalf";
            this.session = session;
            this.bus = bus;
        }

        public void Start()
        {
            bus.Publish(new ApplicationActivityMessage("Negotiating with Twitter"));
            session.GetAuthorizationUrl(new ActionOnDispatcher<Uri>(onAuthorizationuriAvailable));
        }

        public void TwitterLoginLoaded()
        {
            bus.Publish(new ApplicationActivityMessage("Loading login screen"));
        }

        private void onAuthorizationuriAvailable(Uri uri)
        {
            bus.Publish(new ApplicationActivityMessage("Loading login screen"));
            SourceUrl = uri;
            NotifyOfPropertyChange(()=>SourceUrl);
        }

        public Uri SourceUrl { get; private set; }
        
    }
}