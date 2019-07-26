using System;
using System.Threading.Tasks;

namespace SmartRoadSense
{
    public class TrackDataService
    {
        ITrackDataService _trackService;

        public TrackDataService(ITrackDataService trackService)
        {
            _trackService = trackService;
        }

        public Task<Outcome<string, TrackDataException>> SendTracks(string tracks)
        {
            return _trackService.SendTracks(tracks);
        }
    }
}
