using System;
using Caliburn.Micro;
using Membus.WpfTwitterClient.Frame;
using Membus.WpfTwitterClient.Frame.Twitter;

namespace Membus.WpfTwitterClient.GatherAccessToken
{
    public class GetAccessTokenViewModel : Screen
    {
        private readonly ITwitterSession session;

        public GetAccessTokenViewModel(ITwitterSession session)
        {
            DisplayName = "Authorize Membus OnTweet to interact with Twitter on your behalf";
            this.session = session;
        }

        public void Start()
        {
            session.GetAuthorizationUrl(new ActionOnDispatcher<Uri>(onAuthorizationuriAvailable));
        }

        public void TwitterLoginloaded()
        {
            
        }

        private void onAuthorizationuriAvailable(Uri uri)
        {
            SourceUrl = uri;
            NotifyOfPropertyChange(()=>SourceUrl);
        }

        public Uri SourceUrl { get; private set; }
        
    }
}