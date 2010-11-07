using System;
using System.Collections.Generic;
using Membus.WpfTwitterClient.Frame.Config;
using Twitterizer;

namespace Membus.WpfTwitterClient.Frame.Twitter
{
    public interface ITwitterSession
    {
        void GetAuthorizationUrl(Action<Uri> authorizationUriAvailable);
        void GetAccessToken(string verifyCode, Action<TwitterAccessToken> accessTokenAvailable);
        void LoadHomeTimeline(Action<ICollection<TwitterStatus>> timelineAvailable);
    }
}