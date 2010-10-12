using System;

namespace Membus.WpfTwitterClient.Frame.Twitter
{
    public interface ITwitterSession
    {
        void GetAuthorizationUrl(Action<Uri> authorizationUriAvailable);
        void GetAccessToken(string verifyCode, Action<string> accessTokenAvailable);
    }
}