namespace SmartRoadSense.Shared {
    public interface ILevelManager {
        int LevelCount { get; set; }
        int SelectedLevelId { get; set; }
        LevelModel SelectedLevelModel { get; set; }
        LastPlayedLevel LastPlayedLevelInfo { get; set; }
        LevelsContainerModel Levels { get; set; }
        LevelModel LoadSingleLevel(int levelId);
    }
}
