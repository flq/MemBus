using Membus.WpfTwitterClient.Frame.Config;

namespace Membus.WpfTwitterClient.Frame
{
    public interface IUserSettings
    {
        bool IsAccessTokenAvailable { get; }
        TwitterAccessToken AccessToken { get; }
        void StoreAccessToken(TwitterAccessToken token);
    }
}