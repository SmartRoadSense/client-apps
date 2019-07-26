using System;
namespace SmartRoadSense
{
    public class RemoteDataManager
    {
        static RemoteDataManager instance;

        // Service instances
        public TrackDataService TrackDataService { private set; get; }

        // Init
        RemoteDataManager()
        {
            TrackDataService = new TrackDataService(new TrackDataRemote(new Client()));
        }

        // Singletor pattern
        public static RemoteDataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RemoteDataManager();
                }
                return instance;
            }
        }
    }
}
