namespace Membus.WpfTwitterClient.Frame.Config
{
    public class TwitterAccessToken
    {
        private readonly string token;
        private readonly string secret;

        public TwitterAccessToken(string token, string secret)
        {
            this.token = token;
            this.secret = secret;
        }

        public string Secret
        {
            get { return secret; }
        }

        public string Token
        {
            get { return token; }
        }
    }
}