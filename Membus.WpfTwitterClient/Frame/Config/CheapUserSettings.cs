using System;
using System.IO;

namespace Membus.WpfTwitterClient.Frame.Config
{
    public class CheapUserSettings : IUserSettings
    {
        public CheapUserSettings()
        {
            if (!File.Exists("user.txt"))
                File.CreateText("user.txt");
            using (var r = File.OpenText("user.txt"))
                AccessToken = r.ReadToEnd();
        }

        public string AccessToken { get; private set; }

        public void StoreAccessToken(string token)
        {
            AccessToken = token;
            File.WriteAllText("user.txt", AccessToken);
        }
    }
}