using SmartRoadSense.iOS;

namespace SmartRoadSense.Shared
{

	public partial class SyncManager
	{

		private bool CheckPlatformSyncConditions()
		{
            if (Reachability.InternetConnectionStatus() == NetworkStatus.NotReachable)
			{
				Log.Debug("Can't sync: no available connection");
				return false;
			}

			return true;
		}

	}

}