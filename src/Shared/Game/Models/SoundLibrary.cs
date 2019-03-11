using System;
namespace SmartRoadSense.Shared {
    public struct SoundLibrary {
        readonly static string BaseSoundsPath = "Sounds/";

        public struct SFX {
            readonly static string RootPath = BaseSoundsPath + "SFX/";

            // Gameplay
            public readonly static string Coin = RootPath + "coin.wav";
        }

        public struct Music {
            readonly static string RootPath = BaseSoundsPath + "Music/";

            // Menu
            public readonly static string Menu = RootPath + "music_menu.wav";

            // Gameplay
            public readonly static string Beach = RootPath + "music_beach.wav";
            public readonly static string Moon = RootPath + "music_moon.wav";
            public readonly static string SnowyForest = RootPath + "music_snowy_forest.wav";
        }
    }
}
