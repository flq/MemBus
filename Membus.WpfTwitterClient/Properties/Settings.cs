using System;
using Membus.WpfTwitterClient.Frame;

namespace Membus.WpfTwitterClient.Properties
{
    internal partial class Settings : IUserSettings
    {
        public void StoreAccessToken(string token)
        {
            AccessToken = token;
            Save();
        }
    }
}