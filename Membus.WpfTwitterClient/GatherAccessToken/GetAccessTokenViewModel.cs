using Caliburn.Micro;

namespace Membus.WpfTwitterClient.GatherAccessToken
{
    public class GetAccessTokenViewModel : Screen, IHaveDisplayName
    {
        public GetAccessTokenViewModel()
        {
            DisplayName = "Authorize Membus OnTweet to interact with Twitter on your behalf";
        }
    }
}