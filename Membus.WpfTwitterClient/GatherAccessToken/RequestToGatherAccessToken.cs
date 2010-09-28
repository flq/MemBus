using Membus.WpfTwitterClient.Frame.Twitter;

namespace Membus.WpfTwitterClient.GatherAccessToken
{
    public class RequestToGatherAccessToken
    {
        public TwitterKeys Keys { get; private set; }

        public RequestToGatherAccessToken(TwitterKeys keys)
        {
            Keys = keys;
        }
    }
}