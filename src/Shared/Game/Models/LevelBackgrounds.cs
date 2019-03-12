using Urho.Urho2D;
namespace SmartRoadSense.Shared {
    public enum LevelBackgrounds {
        RANDOM = 0,
        MOON = 1,
        SNOWY_FOREST = 2,
        BEACH = 3
    }

    public class LevelBackgroundResourceData {
        public Sprite2D ForegroundSprite { get; private set; }
        public Sprite2D Bg1Sprite { get; private set; }
        public Sprite2D Bg2Sprite { get; private set; }
        public Sprite2D Bg3Sprite { get; private set; }

        public LevelBackgroundResourceData(Game GameInstance, int landscape, int idx) {
            var fgPath = "";
            var bg1Path = "";
            var bg2Path = "";
            var bg3Path = "";

            LevelBackgrounds bg;
            switch(landscape) {
                case 1:
                    bg = LevelBackgrounds.BEACH;
                    break;
                case 2:
                    bg = LevelBackgrounds.MOON;
                    break;
                case 3:
                    bg = LevelBackgrounds.SNOWY_FOREST;
                    break;
                default:
                    bg = LevelBackgrounds.BEACH;
                    break;
            }

            switch(bg) {
                case LevelBackgrounds.BEACH:
                    fgPath = AssetsCoordinates.Backgrounds.Beach.Foreground.Path;
                    bg1Path = AssetsCoordinates.Backgrounds.Beach.Background1.Path;
                    bg2Path = AssetsCoordinates.Backgrounds.Beach.Background2.Path;
                    bg3Path = AssetsCoordinates.Backgrounds.Beach.Background3.Path;
                    break;
                case LevelBackgrounds.MOON:
                    fgPath = AssetsCoordinates.Backgrounds.Moon.Foreground.Path;
                    bg1Path = AssetsCoordinates.Backgrounds.Moon.Background1.Path;
                    bg2Path = AssetsCoordinates.Backgrounds.Moon.Background2.Path;
                    bg3Path = AssetsCoordinates.Backgrounds.Moon.Background3.Path;
                    break;
                case LevelBackgrounds.SNOWY_FOREST:
                    fgPath = AssetsCoordinates.Backgrounds.SnowyForest.Foreground.Path;
                    bg1Path = AssetsCoordinates.Backgrounds.SnowyForest.Background1.Path;
                    bg2Path = AssetsCoordinates.Backgrounds.SnowyForest.Background2.Path;
                    bg3Path = AssetsCoordinates.Backgrounds.SnowyForest.Background3.Path;
                    break;
            }

            fgPath += AssetsCoordinates.Backgrounds.GameplayBackgroundsGeneric.Foreground.Prefix;
            fgPath += idx;
            fgPath += AssetsCoordinates.Backgrounds.GameplayBackgroundsGeneric.Foreground.Suffix;

            bg1Path += AssetsCoordinates.Backgrounds.GameplayBackgroundsGeneric.Background1.Prefix;
            bg1Path += idx;
            bg1Path += AssetsCoordinates.Backgrounds.GameplayBackgroundsGeneric.Background1.Suffix;

            bg2Path += AssetsCoordinates.Backgrounds.GameplayBackgroundsGeneric.Background2.Prefix;
            bg2Path += idx;
            bg2Path += AssetsCoordinates.Backgrounds.GameplayBackgroundsGeneric.Background2.Suffix;

            bg3Path += AssetsCoordinates.Backgrounds.GameplayBackgroundsGeneric.Background3.Prefix;
            bg3Path += idx;
            bg3Path += AssetsCoordinates.Backgrounds.GameplayBackgroundsGeneric.Background3.Suffix;

            ForegroundSprite = GameInstance.ResourceCache.GetSprite2D(fgPath);
            Bg1Sprite = GameInstance.ResourceCache.GetSprite2D(bg1Path);
            Bg2Sprite = GameInstance.ResourceCache.GetSprite2D(bg2Path);
            Bg3Sprite = GameInstance.ResourceCache.GetSprite2D(bg3Path);
        }
    }
}
