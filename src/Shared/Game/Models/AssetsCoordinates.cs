using System;
using Urho;

namespace SmartRoadSense.Shared 
{
    public struct AssetsCoordinates
    {
        readonly static string BaseTexturesAssetsPath = "Textures/";

        public struct Generic
        {
            readonly static string GenericAssetsPath = BaseTexturesAssetsPath + "Generic/";

            public struct TopBar
            {
                readonly static public string ResourcePath = GenericAssetsPath + "top_bar2.png";
                readonly static public IntRect Rectangle = new IntRect(0, 0, 1024, 64);
            }

            public struct Icons {
                readonly static public string ResourcePath = GenericAssetsPath + "round_icons.png";

                //generic
                readonly static public IntRect IconCoin = new IntRect(0, 0, 160, 160);
                readonly static public IntRect IconLocked = new IntRect(200, 0, 360, 160);
                readonly static public IntRect BntBack = new IntRect(1400, 0, 1560, 160);

                //only garage
                readonly static public IntRect TrafficLightRed = new IntRect(400, 0, 560, 160);
                readonly static public IntRect TrafficLightYellow = new IntRect(600, 0, 760, 160);
                readonly static public IntRect TrafficLightGreen = new IntRect(800, 0, 960, 160);
                readonly static public IntRect BtnLeft = new IntRect(1000, 0, 1160, 160);
                readonly static public IntRect BtnRight = new IntRect(1200, 0, 1360, 160);

                //only level
                readonly static public IntRect BtnPlay = new IntRect(1600, 0, 1760, 160);
                readonly static public IntRect BoostIcon1 = new IntRect(1800, 0, 1960, 160);
                readonly static public IntRect BoostIcon2 = new IntRect(0, 200, 160, 360);
                readonly static public IntRect BoostIcon3 = new IntRect(200, 200, 360, 360);
                readonly static public IntRect AButton = new IntRect(400, 200, 560, 360);
                readonly static public IntRect BButton = new IntRect(600, 200, 760, 360);
                readonly static public IntRect BestTimeIcon = new IntRect(800, 200, 960, 360);
                readonly static public IntRect BoosterIcon = new IntRect(1000, 200, 1160, 360);
                readonly static public IntRect CoinsIcon = new IntRect(1200, 200, 1360, 360);
                readonly static public IntRect ComponentsIcon = new IntRect(1400, 200, 1560, 360);
                readonly static public IntRect PauseButton = new IntRect(1600, 200, 1760, 360);
                readonly static public IntRect RLeftButton = new IntRect(1800, 200, 1960, 360);
                readonly static public IntRect RRightButton = new IntRect(0, 400, 160, 560);
                readonly static public IntRect TimeIcon = new IntRect(200, 400, 360, 560);
                readonly static public IntRect BtnSettings = new IntRect(400, 400, 560, 560);
                readonly static public IntRect Continue = new IntRect(600, 400, 760, 560);
                readonly static public IntRect Restart = new IntRect(800, 400, 960, 560);
            }

            public struct Boxes {
                readonly static public string ResourcePath = GenericAssetsPath + "buttons.png";

                //trasparent png - use for containers
                readonly static public IntRect ContainerTrasparent = new IntRect(0, 2000, 40, 2040);

                //generic
                readonly static public IntRect BoxTitle = new IntRect(0, 0, 512, 100);
                readonly static public IntRect BoxConfirmation = new IntRect(0, 920, 1022, 1170);
                readonly static public IntRect SelectionPositive = new IntRect(0, 400, 306, 540);
                readonly static public IntRect SelectionNegative = new IntRect(340, 400, 645, 540);

                //only character
                readonly static public IntRect GroupSelected = new IntRect(550, 0, 810, 110);
                readonly static public IntRect GroupUnselected = new IntRect(850, 0, 1110, 110);
                readonly static public IntRect NameEntry = new IntRect(1150, 0, 1500, 110);

                //only garage
                readonly static public IntRect BoxPerformanceUpgrade = new IntRect(0, 680, 555, 776);
                readonly static public IntRect BoxSuspensionUpgrade = new IntRect(584, 680, 1140, 776);
                readonly static public IntRect BoxBrakeUpgrade = new IntRect(0, 790, 555, 888);
                readonly static public IntRect BoxWheelUpgrade = new IntRect(1168, 680, 1722, 776);
                readonly static public IntRect BoxPerformanceUpgradeNew = new IntRect(1156, 144, 1718, 234);
                readonly static public IntRect BoxSuspensionUpgradeNew = new IntRect(1156, 250, 1718, 340);
                readonly static public IntRect BoxBrakeUpgradeNew = new IntRect(1156, 360, 1718, 450);
                readonly static public IntRect BoxWheelUpgradeNew = new IntRect(1156, 467, 1718, 557);

                readonly static public IntRect BarBoosterDropDown = new IntRect(588, 790, 1430, 888);
                readonly static public IntRect ButtonContinue = new IntRect(700, 400, 1130, 540);
                readonly static public IntRect BoxInstructions = new IntRect(0, 150, 620, 220);
                readonly static public IntRect BoxInfoLocked = new IntRect(296, 570, 985, 673);
                readonly static public IntRect BarUnkockedItem = new IntRect(1040, 570, 1727, 673);
                readonly static public IntRect BtnUpgrade = new IntRect(0, 570, 265, 650);
                readonly static public IntRect ComponentsAlign = new IntRect(2912, 1402, 3938, 1677);
                readonly static public IntRect ComponentBodyGreen = new IntRect(3762, 1696, 3869, 1802);
                readonly static public IntRect ComponentBodyRed = new IntRect(3607, 1696, 3713, 1802);
                readonly static public IntRect ComponentDrivetrainGreen = new IntRect(3451, 1696, 3557, 1802);
                readonly static public IntRect ComponentDrivetrainRed = new IntRect(3295, 1696, 3401, 1802);
                readonly static public IntRect ComponentEngineGreen = new IntRect(3136, 1696, 3243, 1802);
                readonly static public IntRect ComponentEngineRed = new IntRect(2976, 1696, 3083, 1802);


                readonly static public IntRect ComponentSuspensionGreen = new IntRect(3450, 1854, 3557, 1960);
                readonly static public IntRect ComponentSuspensionRed = new IntRect(3300, 1854, 3407, 1960);
                readonly static public IntRect ComponentWhellGreen = new IntRect(3140, 1854, 3248, 1960);
                readonly static public IntRect ComponentWhellRed = new IntRect(2976, 1854, 3083, 1960);



                //only level
                readonly static public IntRect MapBox = new IntRect(0, 1194, 1016, 1360);
                readonly static public IntRect TimeBestBox = new IntRect(0, 1395, 356, 1481);
                readonly static public IntRect ComponentsIcon = new IntRect(398, 1395, 597, 1481);
                readonly static public IntRect CoinsBox = new IntRect(635, 1395, 885, 1481);
                readonly static public IntRect DistanceBox = new IntRect(932, 1395, 1181, 1481);
                readonly static public IntRect MapBar = new IntRect(0, 1683, 10, 1884);
                readonly static public IntRect FinishLine = new IntRect(3960, 0, 2048, 4096);
                readonly static public IntRect BestTimeBox = new IntRect(0, 1500, 602, 1648);
                readonly static public IntRect CurrentTimeBox = new IntRect(648, 1500, 1254, 1648);
                readonly static public IntRect CoinBox = new IntRect(1295, 1500, 1730, 1648);
                readonly static public IntRect ComponentsBox = new IntRect(1772, 1500, 2130, 1648);
                readonly static public IntRect DistanceTextBox = new IntRect(59, 1681, 660, 1884);
                readonly static public IntRect ExitConfirmationBox = new IntRect(1772, 0, 2372, 203);
                readonly static public IntRect PauseMenuBox = new IntRect(1772, 220, 2372, 638);

                //only post-race
                readonly static public IntRect LevelBlueBox = new IntRect(1365, 1395, 1442, 1471);  // used in post-race and user profile
                readonly static public IntRect IconBeach = new IntRect(1773, 780, 2372, 845);
                readonly static public IntRect TimeIconBar = new IntRect(1095, 905, 1690, 975);
                readonly static public IntRect BestIconBar = new IntRect(1095, 995, 1690, 1066); 
                readonly static public IntRect ComponentsIconBar = new IntRect(1095, 1087, 1690, 1156);
                readonly static public IntRect CoinsIconBar = new IntRect(1095, 1178, 1690, 1248);
                readonly static public IntRect PointsIconBar = new IntRect(1095, 1267, 1690, 1338);
                readonly static public IntRect RankRedBox = new IntRect(1509, 1395, 1585, 1471);
                readonly static public IntRect RankIncreaseBar = new IntRect(1773, 872, 2370, 949);
                readonly static public IntRect RaceCompleted = new IntRect(1773, 977, 2370, 1049);

                //only user profile
                readonly static public IntRect IconAchievementsBlue = new IntRect(1642, 1395, 1717, 1471);
                readonly static public IntRect WhiteBackground = new IntRect(2408, 0, 3010, 601);
                readonly static public IntRect BarNameBlue = new IntRect(2408, 643, 3008, 694);

                //only select level
                readonly static public IntRect LevelToComplete = new IntRect(1753, 1410, 2005, 1471);
                readonly static public IntRect LevelMoon = new IntRect(2408, 728, 2808, 1130);
                readonly static public IntRect LevelBeach = new IntRect(2818, 728, 3219, 1130);
                readonly static public IntRect LevelSnow = new IntRect(3238, 728, 3638, 1130);
                readonly static public IntRect LevelBlocked = new IntRect(2408, 1152, 2808, 1554);

                // Loading screen
                readonly static public IntRect LoadingContinueButton = new IntRect(700, 150, 1100, 230);
                readonly static public IntRect LoadingWheel = new IntRect(2150, 1500, 2310, 1660);


                //only settings
                readonly static public IntRect MiniGameScreen = new IntRect(728, 1681, 1329, 2021);
                readonly static public IntRect SelectionBoxGreenPart = new IntRect(2358, 1770, 2580, 1824);
                readonly static public IntRect SelectionBoxWhitePart = new IntRect(2580, 1770, 2958, 1824);
                readonly static public IntRect Credits = new IntRect(1909, 1882, 2510, 2020);

                readonly static public IntRect PausaButton = new IntRect(3056, 1, 3123, 68);

                readonly static public IntRect ButtonsRight = new IntRect(3260, 220, 3460, 595);
                readonly static public IntRect ButtonsLeft = new IntRect(3036, 220, 3239, 595);

                readonly static public IntRect AccBrakeRight = new IntRect(1658, 1678, 1862, 1882);
                readonly static public IntRect AccBrakeLeft = new IntRect(1390, 1678, 1594, 1882);

                //volum bar - parts
                readonly static public IntRect VolumeBarGreen = new IntRect(2362, 1678, 2533, 1741);
                readonly static public IntRect VolumeBarWhite = new IntRect(2533, 1678, 2890, 1741);
                readonly static public IntRect VolumeBarMAX = new IntRect(2890, 1678, 2963, 1741);
                readonly static public IntRect VolumeBarKnob = new IntRect(2309, 1678, 2329, 1741);


                //SOCIAL
                readonly static public IntRect Facebook = new IntRect(1912, 1681, 2062, 1831);
                readonly static public IntRect Tweeter = new IntRect(2112, 1681, 2262, 1831);





            }
        }

        public struct Logos {
            readonly static string AssetPath = BaseTexturesAssetsPath + "Logos/";

            public struct Logo {
                readonly static public string ResourcePath = AssetPath + "logo.png";
                readonly static public IntRect LogoBT = new IntRect(0, 0, 879, 512);
            }

            public struct LevelFailed {
                readonly static public string ResourcePath = AssetPath + "level_failed.png";
                readonly static public IntRect LevelFailedBT = new IntRect(0, 0, 729, 424);
            }

            public struct LevelComplete {
                readonly static public string ResourcePath = AssetPath + "level_complete.png";
                readonly static public IntRect LevelCompleteBT = new IntRect(0, 0, 1024, 424);
            }
        }

        public struct Level
        {
            readonly static string AssetPath = BaseTexturesAssetsPath + "Level/";

            public struct FinishLine
            {
                readonly static public string ResourcePath = AssetPath + "osd_finish_line.png";
                readonly static public IntRect Rectangle = new IntRect(0, 0, 96, 1920);
            }

            public struct BalanceObjects {
                readonly static public string ResourcePath = AssetPath + "balance_objects.png";

                readonly static public IntRect JukeBox = new IntRect(0, 0, 50, 91);
            }

            public struct Collectible {
                readonly static public string ResourcePath = AssetPath + "collectible_assets.png";

                readonly static public IntRect Coin = new IntRect(0, 0, 50, 50);
                readonly static public IntRect Wheels = new IntRect(50, 0, 100, 50);
                readonly static public IntRect Brakes = new IntRect(100, 0, 150, 50);
                readonly static public IntRect Performance = new IntRect(150, 0, 200, 50);
                readonly static public IntRect Suspension = new IntRect(200, 0, 250, 50);
            }
        }

        public struct Settings 
        {
            readonly static string AssetPath = BaseTexturesAssetsPath + "Settings/settings.png";

            public struct Items {
                readonly static public IntRect GameScreen = new IntRect(0, 0, 960, 543);
            }
        }

        public struct Backgrounds {
            readonly static string AssetPath = BaseTexturesAssetsPath + "Backgrounds/";

            public struct FixedBackground {
                public readonly static string ResourcePath = AssetPath + "MenuBackground.png";
                public readonly static IntRect ImageRect = new IntRect(0, 0, 1920, 1080);
            }

            public struct LoadingGameScreen {
                public readonly static string ResourcePath = AssetPath + "loading_screen.png";
                public readonly static IntRect ImageRect = new IntRect(0, 0, 1334, 750);
            }

            public struct LoadingScreen {
                readonly static string ResourcePath = AssetPath + "Loading/";

                public struct Foreground {
                    public readonly static string Path = ResourcePath + "background_f.png";
                    public readonly static IntRect ImageRect = new IntRect(0, 0, 1920, 1080);
                }

                public struct Background1 {
                    public readonly static string Path = ResourcePath + "background_b1.png";
                    public readonly static IntRect ImageRect = new IntRect(0, 0, 1920, 1080);
                }

                public struct Background2 {
                    public readonly static string Path = ResourcePath + "background_b2.png";
                    public readonly static IntRect ImageRect = new IntRect(0, 0, 1920, 1080);
                }

                public struct Background3 {
                    public readonly static string Path = ResourcePath + "background_b3.png";
                    public readonly static IntRect ImageRect = new IntRect(0, 0, 1920, 1080);
                }

                public struct Posters {
                    public readonly static string Path = ResourcePath + "posters.png";
                    public readonly static IntRect Environment = new IntRect(0, 0, 500, 636);
                    public readonly static IntRect Climate = new IntRect(500, 0, 1000, 636);
                    public readonly static IntRect Resources = new IntRect(1000, 0, 1500, 636);
                    public readonly static IntRect Wild = new IntRect(1500, 0, 2000, 636);
                    public readonly static IntRect Waste = new IntRect(2000, 0, 2500, 636);
                }
            }

            public struct GameplayBackgroundsGeneric {

                public struct Foreground {
                    public readonly static string Prefix = "fg_";
                    public readonly static string Suffix = ".png";
                    public readonly static IntRect ImageRect = new IntRect(0, 0, 1334, 750);
                }

                public struct Background1 {
                    public readonly static string Prefix = "bg1_";
                    public readonly static string Suffix = ".png";
                    public readonly static IntRect ImageRect = new IntRect(0, 0, 1334, 750);
                }

                public struct Background2 {
                    public readonly static string Prefix = "bg2_";
                    public readonly static string Suffix = ".png";
                    public readonly static IntRect ImageRect = new IntRect(0, 0, 1334, 750);
                }

                public struct Background3 {
                    public readonly static string Prefix = "bg3_";
                    public readonly static string Suffix = ".png";
                    public readonly static IntRect ImageRect = new IntRect(0, 0, 1334, 750);
                }
            }

            public struct Moon {
                readonly static string ResourcePath = AssetPath + "moon/";

                public struct Foreground {
                    public readonly static string Path = ResourcePath + "fg/";
                }

                public struct Background1 {
                    public readonly static string Path = ResourcePath + "bg1/";
                }

                public struct Background2 {
                    public readonly static string Path = ResourcePath + "bg2/";
                }

                public struct Background3 {
                    public readonly static string Path = ResourcePath + "bg3/";
                }
            }

            public struct Beach {
                readonly static string ResourcePath = AssetPath + "beach/";

                public struct Foreground {
                    public readonly static string Path = ResourcePath + "fg/";
                }

                public struct Background1 {
                    public readonly static string Path = ResourcePath + "bg1/";
                }

                public struct Background2 {
                    public readonly static string Path = ResourcePath + "bg2/";
                }

                public struct Background3 {
                    public readonly static string Path = ResourcePath + "bg3/";
                }
            }

            public struct SnowyForest {
                readonly static string ResourcePath = AssetPath + "snowy_forest/";

                public struct Foreground {
                    public readonly static string Path = ResourcePath + "fg/";
                }

                public struct Background1 {
                    public readonly static string Path = ResourcePath + "bg1/";
                }

                public struct Background2 {
                    public readonly static string Path = ResourcePath + "bg2/";
                }

                public struct Background3 {
                    public readonly static string Path = ResourcePath + "bg3/";
                }
            }
        }
    }
}
