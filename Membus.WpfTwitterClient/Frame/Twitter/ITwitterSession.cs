using System;

namespace Membus.WpfTwitterClient.Frame.Twitter
{
    public interface ITwitterSession
    {
        void GetAuthorizationUrl(Action<Uri> callback);
    }
}