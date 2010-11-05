using System;
using System.Threading.Tasks;
using Twitterizer;

namespace Membus.WpfTwitterClient.Frame.Twitter
{
    public class TwitterSession : ITwitterSession
    {
        private readonly TwitterKeys keys;
        private readonly Func<IUserSettings> userSettingsLoader;
        private string requestToken;
        private string accessToken;

        public TwitterSession(TwitterKeys keys, Func<IUserSettings> userSettingsLoader)
        {
            this.keys = keys;
            this.userSettingsLoader = userSettingsLoader;
        }

        public void GetAuthorizationUrl(Action<Uri> authorizationUriAvailable)
        {
            var t = new Task(
                () =>
                    {
                        var response = OAuthUtility.GetRequestToken(keys.ConsumerKey, keys.ConsumerSecret, "oob");
                        requestToken = response.Token;
                        var uri = OAuthUtility.BuildAuthorizationUri(requestToken);
                        authorizationUriAvailable(uri);
                    });
            t.Start();
        }

        public void GetAccessToken(string verifyCode, Action<string> accessTokenAvailable)
        {
            var t = new Task(
                () =>
                {
                    var response = OAuthUtility.GetAccessToken(keys.ConsumerKey, keys.ConsumerSecret, requestToken, verifyCode);
                    accessToken = response.Token;
                    accessTokenAvailable(accessToken);
                });
            t.Start();
        }
    }
}