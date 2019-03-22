using Urho;
using Urho.Gui;
using Urho.Resources;


namespace SmartRoadSense.Shared {
    public class SceneUserProfile : BaseScene {
        ScreenInfoRatio dim; //variabile rapporto dimensioni schermo
        ResourceCache cache;
        Sprite backgroundSprite;
        Sprite container;
        Sprite black_bar;
        UIElement root;

        UI ui;
        Font font;
        int counter;

        public SceneUserProfile(Game game) : base(game) {
            dim = GameInstance.ScreenInfo;
            root = GameInstance.UI.Root;
            cache = GameInstance.ResourceCache;
            font = cache.GetFont(GameInstance.defaultFont);
            ui = GameInstance.UI;
            JsonReaderCharacter.GetCharacterConfig();
            counter = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("img_character_id", 0);
            CreateUI();
        }

        public void CreateUI() {
            CreateBackground();
            CreateTopBar();
            CreateProfileBar();
            CreateUserHistoryBar();
        }

        void CreateBackground() {
            //TODO: animated background
            var backgroundTexture = cache.GetTexture2D("Textures/MenuBackground.png");
            if(backgroundTexture == null)
                return;
            backgroundSprite = root.CreateSprite();
            backgroundSprite.Texture = backgroundTexture;
            backgroundSprite.SetSize((int)(dim.XScreenRatio * 1920), (int)(dim.YScreenRatio * 1080));
            backgroundSprite.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            backgroundSprite.SetPosition(0, 0);
        }

        void CreateTopBar() {
            var generic_bts = cache.GetTexture2D("Textures/Generic/generic_btn.png");

            black_bar = root.CreateSprite();
            root.AddChild(black_bar);
            black_bar.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.TopBar.ResourcePath);
            black_bar.Opacity = 0.5f;
            black_bar.SetPosition(0, (int)(dim.YScreenRatio * 30));
            black_bar.SetSize((int)(dim.XScreenRatio * 2000), (int)(dim.YScreenRatio * 140));
            black_bar.ImageRect = AssetsCoordinates.Generic.TopBar.Rectangle;

            // BACK
            Button btn_back = new Button();
            root.AddChild(btn_back);
            btn_back.SetStyleAuto(null);
            btn_back.SetPosition((int)(dim.XScreenRatio * 40), (int)(dim.YScreenRatio * 40));
            btn_back.SetSize((int)(dim.XScreenRatio * 120), (int)(dim.YScreenRatio * 120));
            btn_back.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            btn_back.ImageRect = AssetsCoordinates.Generic.Icons.BntBack;
            btn_back.Pressed += args => {
                GameInstance.LaunchScene(GameScenesEnumeration.MENU);
            };


            //COINS
            Button coins = new Button();
            root.AddChild(coins);
            coins.SetStyleAuto(null);
            coins.SetPosition((int)(dim.XScreenRatio * 180), (int)(dim.YScreenRatio * 60));
            coins.SetSize((int)(dim.XScreenRatio * 75), (int)(dim.YScreenRatio * 70));
            coins.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            coins.ImageRect = AssetsCoordinates.Generic.Icons.IconCoin;


            //Wallet text
            Text wallet = new Text();
            coins.AddChild(wallet);
            wallet.SetPosition((int)(dim.XScreenRatio * 90), (int)(dim.YScreenRatio * 10));
            wallet.SetFont(font, dim.XScreenRatio * 30);
            int wallet_tot = CharacterManager.Instance.Wallet;

            wallet.Value = "" + wallet_tot;

            // SCREEN TITLE
            Button screen_title = new Button();
            root.AddChild(screen_title);
            screen_title.SetStyleAuto(null);
            screen_title.SetPosition((int)(dim.XScreenRatio * 1500), (int)(dim.YScreenRatio * 50));
            screen_title.SetSize((int)(dim.XScreenRatio * 400), (int)(dim.YScreenRatio * 100));
            screen_title.Texture = generic_bts;
            screen_title.Enabled = false;
            screen_title.ImageRect = new IntRect(0, 110, 513, 213);
            Text buttonTitleText = new Text();
            screen_title.AddChild(buttonTitleText);
            buttonTitleText.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            buttonTitleText.SetPosition(0, 0);
            buttonTitleText.SetFont(font, dim.XScreenRatio * 30);
            buttonTitleText.Value = "PROFILE";
            
        }

        void CreateProfileBar() {
            container = root.CreateSprite();
            container.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            container.ImageRect = AssetsCoordinates.Generic.Boxes.ContainerTrasparent;
            container.SetSize((int)(dim.XScreenRatio * 1200), (int)(dim.YScreenRatio * 1400));
            container.SetPosition((int)(dim.XScreenRatio * 0), (int)(dim.YScreenRatio * 50));

            // USER LEVEL INDICATOR
            Sprite LevelInd = new Sprite();
            container.AddChild(LevelInd);
            LevelInd.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            LevelInd.ImageRect = AssetsCoordinates.Generic.Boxes.LevelBlueBox;
            LevelInd.SetSize((int)(dim.XScreenRatio * 140), (int)(dim.YScreenRatio * 140));
            LevelInd.SetPosition((int)(dim.XScreenRatio * 80), (int)(dim.YScreenRatio * 220));

            // Title
            Text level = new Text();
            LevelInd.AddChild(level);
            level.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Top);
            level.SetPosition(0, 10);
            level.SetFont(font, dim.XScreenRatio * 20);
            level.Value = "LEVEL";

            // Value
            Text levelnumber = new Text();
            LevelInd.AddChild(levelnumber);
            levelnumber.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            levelnumber.SetPosition(0, 15);
            levelnumber.SetFont(font, dim.XScreenRatio * 50);
            levelnumber.Value = string.Format("{0}", CharacterManager.Instance.User.Level);

            Sprite AchievementsIcon = new Sprite();
            container.AddChild(AchievementsIcon);
            AchievementsIcon.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            AchievementsIcon.ImageRect = AssetsCoordinates.Generic.Boxes.IconAchievementsBlue;
            AchievementsIcon.SetSize((int)(dim.XScreenRatio * 140), (int)(dim.YScreenRatio * 140));
            AchievementsIcon.SetPosition((int)(dim.XScreenRatio * 80), (int)(dim.YScreenRatio * 400));

            Sprite WhiteBox = new Sprite();
            container.AddChild(WhiteBox);
            WhiteBox.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            WhiteBox.ImageRect = AssetsCoordinates.Generic.Boxes.WhiteBackground;
            WhiteBox.SetSize((int)(dim.XScreenRatio * 500), (int)(dim.YScreenRatio * 500));
            WhiteBox.SetPosition((int)(dim.XScreenRatio * 250), (int)(dim.YScreenRatio * 220));

            var character = JsonReaderCharacter.GetSingleCharacter(CharacterManager.Instance.User.PortraitId);
            int left = character.ImagePosition.Left;
            int top = character.ImagePosition.Top;
            int right = character.ImagePosition.Right;
            int bottom = character.ImagePosition.Bottom;

            //set the profile image
            Button ProfileImage = new Button();
            container.AddChild(ProfileImage);
            ProfileImage.Texture = cache.GetTexture2D("Textures/Generic/profiles.png");
            ProfileImage.ImageRect = new IntRect(left, top, right, bottom);
            ProfileImage.SetSize((int)(dim.XScreenRatio * 490), (int)(dim.YScreenRatio * 495));
            ProfileImage.SetPosition((int)(dim.XScreenRatio * 255), (int)(dim.YScreenRatio * 220));
            ProfileImage.Pressed += args => {
                GameInstance.LaunchScene(GameScenesEnumeration.PROFILE);
            };

            var buttonProfileImage = new Button();
            container.AddChild(buttonProfileImage);
            buttonProfileImage.Texture = cache.GetTexture2D("Textures/Generic/profiles.png");
            buttonProfileImage.ImageRect = new IntRect(left, top, right, bottom);
            buttonProfileImage.SetSize((int)(dim.XScreenRatio * 490), (int)(dim.YScreenRatio * 495));
            buttonProfileImage.SetPosition((int)(dim.XScreenRatio * 255), (int)(dim.YScreenRatio * 220));

            buttonProfileImage.Pressed += (PressedEventArgs args) => {
                GameInstance.LaunchScene(GameScenesEnumeration.PROFILE, true);
            };

            // USERNAME 
            Sprite UsernameBar = new Sprite();
            container.AddChild(UsernameBar);
            UsernameBar.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            UsernameBar.ImageRect = AssetsCoordinates.Generic.Boxes.BarNameBlue;
            UsernameBar.SetSize((int)(dim.XScreenRatio * 660), (int)(dim.YScreenRatio * 70));
            UsernameBar.SetPosition((int)(dim.XScreenRatio * 160), (int)(dim.YScreenRatio * 800));

            // Valye
            Text Username = new Text();
            UsernameBar.AddChild(Username);
            Username.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            Username.SetPosition(0, 0);
            Username.SetFont(font, dim.XScreenRatio * 30);
            Username.Value = CharacterManager.Instance.User.Username;
            
        }

        void CreateUserHistoryBar() {
            container = root.CreateSprite();
            container.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            container.ImageRect = AssetsCoordinates.Generic.Boxes.ContainerTrasparent;
            container.SetSize((int)(dim.XScreenRatio * 900), (int)(dim.YScreenRatio * 1400));
            container.SetPosition((int)(dim.XScreenRatio * 900), (int)(dim.YScreenRatio * 50));

            // TOTAL NUMBER OF RACES
            Text NOfRaces = new Text();
            container.AddChild(NOfRaces);
            NOfRaces.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            NOfRaces.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(200));
            NOfRaces.SetFont(font, dim.XScreenRatio * 30);
            NOfRaces.Value = "No. of Races:";

            // Value
            Text NOfRacesValue = new Text();
            container.AddChild(NOfRacesValue);
            NOfRacesValue.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            NOfRacesValue.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(200));
            NOfRacesValue.SetFont(font, dim.XScreenRatio * 30);
            NOfRacesValue.Value = string.Format("{0}", LevelManager.Instance.PlayedLevels);

            // COMPLETED RACES NUMBER
            Text RacesCompleted = new Text();
            container.AddChild(RacesCompleted);
            RacesCompleted.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            RacesCompleted.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(260));
            RacesCompleted.SetFont(font, dim.XScreenRatio * 30);
            RacesCompleted.Value = "Races Completed:";

            // Value
            Text RacesCompletedValue = new Text();
            container.AddChild(RacesCompletedValue);
            RacesCompletedValue.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            RacesCompletedValue.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(260));
            RacesCompletedValue.SetFont(font, dim.XScreenRatio * 30);
            RacesCompletedValue.Value = string.Format("{0}", LevelManager.Instance.CompletedLevels);

            // FAILED RACES NUMBER
            Text RacesFailed = new Text();
            container.AddChild(RacesFailed);
            RacesFailed.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            RacesFailed.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(320));
            RacesFailed.SetFont(font, dim.XScreenRatio * 30);
            RacesFailed.Value = "Races Failed:";

            // Value
            Text RacesFailedValue = new Text();
            container.AddChild(RacesFailedValue);
            RacesFailedValue.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            RacesFailedValue.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(320));
            RacesFailedValue.SetFont(font, dim.XScreenRatio * 30);
            RacesFailedValue.Value = string.Format("{0}", LevelManager.Instance.FailedLevels);

            // RACE COMPLETION RATIO
            Text RacesCompletitionRatio = new Text();
            container.AddChild(RacesCompletitionRatio);
            RacesCompletitionRatio.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            RacesCompletitionRatio.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(380));
            RacesCompletitionRatio.SetFont(font, dim.XScreenRatio * 30);
            RacesCompletitionRatio.Value = "Races Completition Ratio:";

            // Value
            Text RacesCompletitionRatioValue = new Text();
            container.AddChild(RacesCompletitionRatioValue);
            RacesCompletitionRatioValue.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            RacesCompletitionRatioValue.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(380));
            RacesCompletitionRatioValue.SetFont(font, dim.XScreenRatio * 30);
            RacesCompletitionRatioValue.Value = string.Format("{0} %", LevelManager.Instance.CompletedPercentage.ToString("n2"));

            // MOST SINGLE RACE POINTS
            Text MostPoints = new Text();
            container.AddChild(MostPoints);
            MostPoints.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            MostPoints.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(440));
            MostPoints.SetFont(font, dim.XScreenRatio * 30);
            MostPoints.Value = "Most Points - Single Race:";

            // Value
            Text MostPointsValue = new Text();
            container.AddChild(MostPointsValue);
            MostPointsValue.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            MostPointsValue.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(440));
            MostPointsValue.SetFont(font, dim.XScreenRatio * 30);
            MostPointsValue.Value = string.Format("{0}", LevelManager.Instance.MostPointsInSingleRace);

            // EXPERIENCE
            Text TotalPoints = new Text();
            container.AddChild(TotalPoints);
            TotalPoints.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            TotalPoints.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(500));
            TotalPoints.SetFont(font, dim.XScreenRatio * 30);
            TotalPoints.Value = "Total Points:";

            // Value
            Text TotalPointsValue = new Text();
            container.AddChild(TotalPointsValue);
            TotalPointsValue.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            TotalPointsValue.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(500));
            TotalPointsValue.SetFont(font, dim.XScreenRatio * 30);
            TotalPointsValue.Value = string.Format("{0}", CharacterManager.Instance.User.Experience);

            // COINS
            Text CoinsCollected = new Text();
            container.AddChild(CoinsCollected);
            CoinsCollected.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            CoinsCollected.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(560));
            CoinsCollected.SetFont(font, dim.XScreenRatio * 30);
            CoinsCollected.Value = "Coins Collected:";

            // Value
            Text CoinsCollectedValue = new Text();
            container.AddChild(CoinsCollectedValue);
            CoinsCollectedValue.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            CoinsCollectedValue.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(560));
            CoinsCollectedValue.SetFont(font, dim.XScreenRatio * 30);
            CoinsCollectedValue.Value = string.Format("{0}", CharacterManager.Instance.User.Wallet);

            // VEHICLES OWNED
            /*
            Text VehiclesOwned = new Text();
            container.AddChild(VehiclesOwned);
            VehiclesOwned.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            VehiclesOwned.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(620));
            VehiclesOwned.SetFont(font, dim.XScreenRatio * 30);
            VehiclesOwned.Value = "Vehicles Owned:";

            // Value
            Text VehiclesOwnedValue = new Text();
            container.AddChild(VehiclesOwnedValue);
            VehiclesOwnedValue.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            VehiclesOwnedValue.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(620));
            VehiclesOwnedValue.SetFont(font, dim.XScreenRatio * 30);
            VehiclesOwnedValue.Value = "TODO";
            */
            // UNLOCKED VEHICLES 
            /*
            Text VehiclesUnlocked = new Text();
            container.AddChild(VehiclesUnlocked);
            VehiclesUnlocked.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            VehiclesUnlocked.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(680));
            VehiclesUnlocked.SetFont(font, dim.XScreenRatio * 30);
            VehiclesUnlocked.Value = "Vehicles Unlocked:";

            // Value
            Text VehiclesUnlockedValue = new Text();
            container.AddChild(VehiclesUnlockedValue);
            VehiclesUnlockedValue.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            VehiclesUnlockedValue.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(680));
            VehiclesUnlockedValue.SetFont(font, dim.XScreenRatio * 30);
            VehiclesUnlockedValue.Value = string.Format("{0}", VehicleManager.Instance.VehiclesUnlocked);
            */
/*
            // COMPONENTS COLLECTED
            Text ComponentsCollected = new Text();
            container.AddChild(ComponentsCollected);
            ComponentsCollected.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            ComponentsCollected.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(740));
            ComponentsCollected.SetFont(font, dim.XScreenRatio * 30);
            ComponentsCollected.Value = "Components Collected:";

            // Value
            Text ComponentsCollectedValue = new Text();
            container.AddChild(ComponentsCollectedValue);
            ComponentsCollectedValue.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            ComponentsCollectedValue.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(740));
            ComponentsCollectedValue.SetFont(font, dim.XScreenRatio * 30);
            ComponentsCollectedValue.Value = "0";

            // BOOSTERS USED
            Text BoostersUsed = new Text();
            container.AddChild(BoostersUsed);
            BoostersUsed.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            BoostersUsed.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(800));
            BoostersUsed.SetFont(font, dim.XScreenRatio * 30);
            BoostersUsed.Value = "Boosters Used:";

            // Value
            Text BoostersUsedValue = new Text();
            container.AddChild(BoostersUsedValue);
            BoostersUsedValue.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
            BoostersUsedValue.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(800));
            BoostersUsedValue.SetFont(font, dim.XScreenRatio * 30);
            BoostersUsedValue.Value = "0";
            */
        }
    }
}
