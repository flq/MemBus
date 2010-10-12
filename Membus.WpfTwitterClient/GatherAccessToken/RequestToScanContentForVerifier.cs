namespace Membus.WpfTwitterClient.GatherAccessToken
{
    public class RequestToScanContentForVerifier
    {
        private readonly string content;

        public RequestToScanContentForVerifier(string content)
        {
            this.content = content;
        }

        public string Content
        {
            get { return content; }
        }
    }
}