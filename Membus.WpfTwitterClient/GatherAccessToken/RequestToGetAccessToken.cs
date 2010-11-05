namespace Membus.WpfTwitterClient.GatherAccessToken
{
    public class RequestToGetAccessToken
    {
        private readonly string pin;

        public RequestToGetAccessToken(string pin)
        {
            this.pin = pin;
        }

        public string Pin
        {
            get { return pin; }
        }
    }
}