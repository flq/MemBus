namespace Membus.WpfTwitterClient.Frame
{
    public interface IUserSettings
    {
        string AccessToken { get; }
        void StoreAccessToken(string token);
    }
}