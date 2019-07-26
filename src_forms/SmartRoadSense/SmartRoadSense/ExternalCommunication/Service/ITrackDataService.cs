using System;
using System.Threading.Tasks;

namespace SmartRoadSense
{
    public interface ITrackDataService
    {
        Task<Outcome<string, TrackDataException>> SendTracks(string tracks);
    }
}
