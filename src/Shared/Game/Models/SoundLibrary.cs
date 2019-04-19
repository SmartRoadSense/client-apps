using System;
namespace SmartRoadSense.Shared {
    public struct SoundLibrary {
        readonly static string BaseSoundsPath = "Sounds/";

        public struct SFX {
            readonly static string RootPath = BaseSoundsPath + "SFX/";

            // Generic
            public readonly static string Swipe = RootPath + "coin.wav"; // TODO
            public readonly static string Next = RootPath + "coin.wav"; // TODO
            public readonly static string Back = RootPath + "coin.wav"; // TODO
            public readonly static string Error = RootPath + "coin.wav"; // TODO
            public readonly static string VehiclePurchase = RootPath + "coin.wav"; // TODO
            public readonly static string UpgradePurchase = RootPath + "coin.wav"; // TODO

            // Gameplay
            public readonly static string Coin = RootPath + "coin.wav";
            public readonly static string Component = RootPath + "coin.wav"; // TODO
        }

        public struct Music {
            readonly static string RootPath = BaseSoundsPath + "Music/";

            // Menu
            public readonly static string Menu = RootPath + "music_menu.wav";
            public readonly static string LevelComplete = RootPath + "music_menu.wav"; // TODO
            public readonly static string LevelFailed= RootPath + "music_menu.wav"; // TODO

            // Gameplay
            public readonly static string Beach = RootPath + "music_beach.wav";
            public readonly static string Moon = RootPath + "music_moon.wav";
            public readonly static string SnowyForest = RootPath + "music_snowy_forest.wav";
        }
    }
}
