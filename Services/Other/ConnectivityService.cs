
namespace oculus_sport.Services.Other
{
    public class ConnectivityService
    {
        public bool IsConnected()
        {
            return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        }
    }
}
