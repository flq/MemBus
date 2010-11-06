using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Membus.WpfTwitterClient.Frame.Config;
using Twitterizer;

namespace Membus.WpfTwitterClient.Frame.Twitter
{
    public class TwitterSession : ITwitterSession
    {
        private readonly TwitterKeys keys;
        private readonly Func<IUserSettings> userSettingsLoader;
        private string requestToken;

        readonly TaskFactory taskFactory = new TaskFactory();

        public TwitterSession(TwitterKeys keys, Func<IUserSettings> userSettingsLoader)
        {
            this.keys = keys;
            this.userSettingsLoader = userSettingsLoader;
        }

        public void GetAuthorizationUrl(Action<Uri> authorizationUriAvailable)
        {
            taskFactory.StartNew(() =>
                                     {
                                         var response = OAuthUtility.GetRequestToken(keys.ConsumerKey,
                                                                                     keys.ConsumerSecret, "oob");
                                         requestToken = response.Token;
                                         var uri = OAuthUtility.BuildAuthorizationUri(requestToken);
                                         authorizationUriAvailable(uri);
                                     });
        }

        public void GetAccessToken(string verifyCode, Action<TwitterAccessToken> accessTokenAvailable)
        {
            taskFactory.StartNew(() =>
                {
                    var response = OAuthUtility.GetAccessToken(keys.ConsumerKey, keys.ConsumerSecret, requestToken, verifyCode);
                    accessTokenAvailable(new TwitterAccessToken(response.Token, response.TokenSecret));
                });
        }

        public void LoadPublicTimeline(Action<ICollection<TwitterStatus>> action)
        {
            taskFactory.StartNew(() =>
                                     {
                                         var statuses = TwitterTimeline.PublicTimeline(getOAuthToken());
                                         action(statuses);
                                     });
        }

        private OAuthTokens getOAuthToken()
        {
            var userSettings = userSettingsLoader();
            if (!userSettings.IsAccessTokenAvailable)
                throw new InvalidOperationException("By some reason no access token is available, this is an invalid state of the application.");
            var token = userSettings.AccessToken;
            return new OAuthTokens
                       {
                           AccessToken = token.Token,
                           AccessTokenSecret = token.Secret,
                           ConsumerKey = keys.ConsumerKey,
                           ConsumerSecret = keys.ConsumerSecret
                       };
        }
    }
}