using System;
using System.IO;

namespace Membus.WpfTwitterClient.Frame.Config
{
    public class CheapUserSettings : IUserSettings
    {
        public CheapUserSettings()
        {
            if (!File.Exists("user.txt"))
                return;
            using (var r = File.OpenText("user.txt"))
            {
                AccessToken = new TwitterAccessToken(r.ReadLine(), r.ReadLine());
            }
        }

        public bool IsAccessTokenAvailable
        {
            get { return AccessToken != null; }
        }

        public TwitterAccessToken AccessToken { get; private set; }

        public void StoreAccessToken(TwitterAccessToken token)
        {
            AccessToken = token;
            File.WriteAllText("user.txt", token.Token + Environment.NewLine + token.Secret);
        }
    }
}