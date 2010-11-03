using System;
using System.Windows.Controls;
using System.Windows.Navigation;
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
        private Action<Uri> loadUri;
        private Func<string> loadedContents;

        public GetAccessTokenViewModel(ITwitterSession session, IBus bus)
        {
            DisplayName = "Authorize Membus OnTweet to interact with Twitter on your behalf";
            this.session = session;
            this.bus = bus;
        }

        public void TwitterLoadCompleted(NavigationEventArgs e)
        {
            var s = loadedContents();
            bus.Publish(new ApplicationActivityMessage());
            bus.Publish(new RequestToScanContentForVerifier(s));
            
        }

        public void BrowserLoaded(WebBrowser browser)
        {
            bus.Publish(new RequestForAttention(new ConfirmationMessage("Test")));
            return;
            // Source property ocannot be bound, which is why we have to do silly things here
            loadUri = uri => browser.Source = uri;
            loadedContents = () =>
                                 {
                                     var b = browser.Document as dynamic;
                                     return (string) b.documentElement.innerText;
                                 };
            bus.Publish(new ApplicationActivityMessage("Negotiating with Twitter"));
            session.GetAuthorizationUrl(new ActionOnDispatcher<Uri>(onAuthorizationuriAvailable));
        }

        public void TwitterWebsiteNavigating(NavigatingCancelEventArgs args)
        {
            bus.Publish(new ApplicationActivityMessage("Loading Twitter screen"));
        }

        private void onAuthorizationuriAvailable(Uri uri)
        {
            loadUri(uri);
        }
        
    }
}