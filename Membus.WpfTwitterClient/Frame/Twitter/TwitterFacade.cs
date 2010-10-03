using System;
using System.Threading.Tasks;
using Membus.WpfTwitterClient.Frame.UI;
using Twitterizer;

namespace Membus.WpfTwitterClient.Frame.Twitter
{
    public class TwitterSession : ITwitterSession
    {
        private readonly TwitterKeys keys;

        public TwitterSession(TwitterKeys keys)
        {
            this.keys = keys;
        }

        public void GetAuthorizationUrl(Action<Uri> callback)
        {
            var t = new Task(
                () =>
                    {
                        var response = OAuthUtility.GetRequestToken(keys.ConsumerKey, keys.ConsumerSecret, "oob");
                        var uri = OAuthUtility.BuildAuthorizationUri(response.Token);
                        callback(uri);
                    });
            t.Start();
        }
    }
}