using System;
using Membus.WpfTwitterClient.Frame;
using Membus.WpfTwitterClient.Frame.Config;

namespace Membus.WpfTwitterClient.Properties
{
    internal partial class Settings //: IUserSettings
    {
        public void StoreAccessToken(TwitterAccessToken token)
        {
            //AccessToken = token;
            Save();
        }
    }
}