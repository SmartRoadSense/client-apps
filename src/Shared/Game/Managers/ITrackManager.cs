namespace SmartRoadSense.Shared {
    public interface ITrackManager {
        int TrackCount { get; }
        int SelectedTrackId { get; set; }
        TrackModel SelectedTrackModel { get; set; }
        LastPlayedTrack LastPlayedTrackInfo { get; set; }
        TracksContainerModel Tracks { get; set; }
        TrackModel LoadSingleLevel(int levelId);
    }
}
