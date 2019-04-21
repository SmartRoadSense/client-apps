using System;
using Urho;
using Urho.Gui;
using Urho.Resources;

namespace SmartRoadSense.Shared 
{
    public class ScenePostRace : BaseScene
	{
        readonly UIElement root;
        readonly ScreenInfoRatio dim; //variabile rapporto dimensioni schermo
        readonly ResourceCache cache;
        readonly Font font;
        readonly TrackModel _levelInfo;

        Sprite black_bar;
        Sprite backgroundSprite;
        Sprite container;
        LastPlayedTrack _postLevelData;

        public ScenePostRace(Game game, bool randomLevel = false) : base(game) 
		{
            dim = GameInstance.ScreenInfo;
            root = GameInstance.UI.Root;
            cache = GameInstance.ResourceCache;
            font = cache.GetFont(GameInstance.defaultFont);
            _levelInfo = TrackManager.Instance.SelectedTrackModel ?? null;

            // Update best time data if not random level
            if(!randomLevel)
                UpdateLevelInfo();
                
            CreateBackground();
            CreateTopBar();
            CreateScene();
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
            backgroundSprite.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(0));
        }

        void CreateTopBar() {

            black_bar = root.CreateSprite();
            GameInstance.UI.Root.AddChild(black_bar);
            black_bar.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.TopBar.ResourcePath);
            black_bar.Opacity = 0.5f;
            black_bar.SetPosition(GameInstance.ScreenInfo.SetX(0), (int)(dim.YScreenRatio * 30));
            black_bar.SetSize((int)(dim.XScreenRatio * 2000), (int)(dim.YScreenRatio * 140));
            black_bar.ImageRect = AssetsCoordinates.Generic.TopBar.Rectangle;

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
            screen_title.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            screen_title.ImageRect = AssetsCoordinates.Generic.Boxes.BoxTitle;
            screen_title.Enabled = false;

            Text buttonTitleText = new Text();
            screen_title.AddChild(buttonTitleText);
            buttonTitleText.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            buttonTitleText.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(0));
            buttonTitleText.SetFont(font, dim.XScreenRatio * 30);
            buttonTitleText.Value = "RESULTS";
        }

        void CreateScene() {
            container = root.CreateSprite();
            container.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            container.ImageRect = AssetsCoordinates.Generic.Boxes.ContainerTrasparent;
            container.SetSize((int)(dim.XScreenRatio * 1200), (int)(dim.YScreenRatio * 1400));
            container.SetPosition((int)(dim.XScreenRatio * 0), (int)(dim.YScreenRatio * 0));

            Sprite LevelInd = new Sprite();
            container.AddChild(LevelInd);
            LevelInd.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            LevelInd.ImageRect = AssetsCoordinates.Generic.Boxes.LevelBlueBox;
            LevelInd.SetSize((int)(dim.XScreenRatio * 140), (int)(dim.YScreenRatio * 140));
            LevelInd.SetPosition((int)(dim.XScreenRatio * 220), (int)(dim.YScreenRatio * 220));

            //STATIC TEXT 
            Text level = new Text();
            LevelInd.AddChild(level);
            level.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Top);
            level.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(10));
            level.SetFont(font, dim.XScreenRatio * 20);
            level.Value = "RACE";

            // CURRENT RACE NUMBER
            Text levelnumber = new Text();
            LevelInd.AddChild(levelnumber);
            levelnumber.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            levelnumber.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(15));
            levelnumber.SetFont(font, dim.XScreenRatio * 50);
            if(_postLevelData != null && _postLevelData.TrackData != null)
                levelnumber.Value = _postLevelData.TrackData.IdTrack.ToString();

            Sprite LevelIcon = new Sprite();
            container.AddChild(LevelIcon);
            //LevelIcon.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            LevelIcon.ImageRect = AssetsCoordinates.Generic.Boxes.IconBeach;
            LevelIcon.SetSize((int)(dim.XScreenRatio * 1200), (int)(dim.YScreenRatio * 140));
            LevelIcon.SetPosition((int)(dim.XScreenRatio * 410), (int)(dim.YScreenRatio * 220));

            Sprite ResultsContainer = new Sprite();
            container.AddChild(ResultsContainer);
            ResultsContainer.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            ResultsContainer.ImageRect = AssetsCoordinates.Generic.Boxes.ContainerTrasparent;
            ResultsContainer.SetSize((int)(dim.XScreenRatio * 1200), (int)(dim.YScreenRatio * 1200));
            ResultsContainer.SetPosition((int)(dim.XScreenRatio * 220), (int)(dim.YScreenRatio * 250));

            // CURRENT TIME
            Sprite TimeIcon = new Sprite();
            ResultsContainer.AddChild(TimeIcon);
            TimeIcon.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            TimeIcon.ImageRect = AssetsCoordinates.Generic.Boxes.TimeIconBar;
            TimeIcon.SetSize((int)(dim.XScreenRatio * 800), (int)(dim.YScreenRatio * 100));
            TimeIcon.SetPosition((int)(dim.XScreenRatio * 350), (int)(dim.YScreenRatio * 120));

            // Title
            Text Time = new Text();
            TimeIcon.AddChild(Time);
            Time.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Center);
            Time.SetPosition(GameInstance.ScreenInfo.SetX(100), GameInstance.ScreenInfo.SetY(0));
            Time.SetFont(font, dim.XScreenRatio * 30);
            Time.Value = "Time:";

            // Time value
            Text TimeTot = new Text();
            TimeIcon.AddChild(TimeTot);
            TimeTot.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Center);
            TimeTot.SetPosition(GameInstance.ScreenInfo.SetX(-30), GameInstance.ScreenInfo.SetY(0));
            TimeTot.SetFont(font, dim.XScreenRatio * 30);
            if(_postLevelData != null && _postLevelData.TrackData != null)
                TimeTot.Value = TimeSpan.FromMilliseconds(_postLevelData.Time).MillisRepresentation();

            // BEST TIME
            Sprite BestIcon = new Sprite();
            ResultsContainer.AddChild(BestIcon);
            BestIcon.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            BestIcon.ImageRect = AssetsCoordinates.Generic.Boxes.BestIconBar;
            BestIcon.SetSize((int)(dim.XScreenRatio * 800), (int)(dim.YScreenRatio * 100));
            BestIcon.SetPosition((int)(dim.XScreenRatio * 350), (int)(dim.YScreenRatio * 230));

            // Title
            Text Best = new Text();
            BestIcon.AddChild(Best);
            Best.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Center);
            Best.SetPosition(GameInstance.ScreenInfo.SetX(100), GameInstance.ScreenInfo.SetY(0));
            Best.SetFont(font, dim.XScreenRatio * 30);
            Best.Value = "Best:";

            // Value
            Text BestTot = new Text();
            BestIcon.AddChild(BestTot);
            BestTot.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Center);
            BestTot.SetPosition(GameInstance.ScreenInfo.SetX(-30), GameInstance.ScreenInfo.SetY(0));
            BestTot.SetFont(font, dim.XScreenRatio * 30);
            if(_postLevelData != null && _postLevelData.TrackData != null)
                BestTot.Value = TimeSpan.FromMilliseconds(_postLevelData.TrackData.BestTime).MillisRepresentation();

            // COMPONENTS
            Sprite ComponentsIcon = new Sprite();
            ResultsContainer.AddChild(ComponentsIcon);
            ComponentsIcon.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            ComponentsIcon.ImageRect = AssetsCoordinates.Generic.Boxes.ComponentsIconBar;
            ComponentsIcon.SetSize((int)(dim.XScreenRatio * 800), (int)(dim.YScreenRatio * 100));
            ComponentsIcon.SetPosition((int)(dim.XScreenRatio * 350), (int)(dim.YScreenRatio * 340));

            // Title
            Text components = new Text();
            ComponentsIcon.AddChild(components);
            components.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Center);
            components.SetPosition(GameInstance.ScreenInfo.SetX(100), GameInstance.ScreenInfo.SetY(0));
            components.SetFont(font, dim.XScreenRatio * 30);
            components.Value = "Components:";

            // Value
            Text ComponentsTot = new Text();
            ComponentsIcon.AddChild(ComponentsTot);
            ComponentsTot.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Center);
            ComponentsTot.SetPosition(GameInstance.ScreenInfo.SetX(-30), GameInstance.ScreenInfo.SetY(0));
            ComponentsTot.SetFont(font, dim.XScreenRatio * 30);
            if(_postLevelData != null && _postLevelData.TrackData != null)
                ComponentsTot.Value = "x" + _postLevelData.Components;

            // COINS
            Sprite CoinsIcon = new Sprite();
            ResultsContainer.AddChild(CoinsIcon);
            CoinsIcon.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            CoinsIcon.ImageRect = AssetsCoordinates.Generic.Boxes.CoinsIconBar;
            CoinsIcon.SetSize((int)(dim.XScreenRatio * 800), (int)(dim.YScreenRatio * 100));
            CoinsIcon.SetPosition((int)(dim.XScreenRatio * 350), (int)(dim.YScreenRatio * 450));

            // Title 
            Text Coins = new Text();
            CoinsIcon.AddChild(Coins);
            Coins.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Center);
            Coins.SetPosition(GameInstance.ScreenInfo.SetX(100), GameInstance.ScreenInfo.SetY(0));
            Coins.SetFont(font, dim.XScreenRatio * 30);
            Coins.Value = "Coins:";

            // Value
            Text CoinsTot = new Text();
            CoinsIcon.AddChild(CoinsTot);
            CoinsTot.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Center);
            CoinsTot.SetPosition(GameInstance.ScreenInfo.SetX(-30), GameInstance.ScreenInfo.SetY(0));
            CoinsTot.SetFont(font, dim.XScreenRatio * 30);
            if(_postLevelData != null && _postLevelData.TrackData != null)
                CoinsTot.Value = "x" + _postLevelData.Coins;

            // POINTS
            Sprite PointsIcon = new Sprite();
            ResultsContainer.AddChild(PointsIcon);
            PointsIcon.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            PointsIcon.ImageRect = AssetsCoordinates.Generic.Boxes.PointsIconBar;
            PointsIcon.SetSize((int)(dim.XScreenRatio * 800), (int)(dim.YScreenRatio * 100));
            PointsIcon.SetPosition((int)(dim.XScreenRatio * 350), (int)(dim.YScreenRatio * 560));

            // Title
            Text Points = new Text();
            PointsIcon.AddChild(Points);
            Points.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Center);
            Points.SetPosition(GameInstance.ScreenInfo.SetX(100), GameInstance.ScreenInfo.SetY(0));
            Points.SetFont(font, dim.XScreenRatio * 30);
            Points.Value = "Points:";

            // Value
            Text PointsTot = new Text();
            PointsIcon.AddChild(PointsTot);
            PointsTot.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Center);
            PointsTot.SetPosition(GameInstance.ScreenInfo.SetX(-30), GameInstance.ScreenInfo.SetY(0));
            PointsTot.SetFont(font, dim.XScreenRatio * 30);
            if(_postLevelData != null && _postLevelData.TrackData != null)
                PointsTot.Value = _postLevelData.Points.ToString();

            // CHARACTER LEVEL
            Sprite RankRadBox1 = new Sprite();
            //container.AddChild(RankRadBox1);
            RankRadBox1.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            RankRadBox1.ImageRect = AssetsCoordinates.Generic.Boxes.RankRedBox;
            RankRadBox1.SetSize((int)(dim.XScreenRatio * 140), (int)(dim.YScreenRatio * 140));
            RankRadBox1.SetPosition((int)(dim.XScreenRatio * 220), (int)(dim.YScreenRatio * 920));

            // Title
            Text rank1 = new Text();
            RankRadBox1.AddChild(rank1);
            rank1.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Top);
            rank1.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(10));
            rank1.SetFont(font, dim.XScreenRatio * 20);
            rank1.Value = "LEVEL";

            // Value
            Text ranknumber = new Text();
            RankRadBox1.AddChild(ranknumber);
            ranknumber.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            ranknumber.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(15));
            ranknumber.SetFont(font, dim.XScreenRatio * 50);
            ranknumber.Value = CharacterManager.Instance.User.Level.ToString();

            // LEVEL BASE BOX
            Sprite RankIncrease = new Sprite();
            //container.AddChild(RankIncrease);
            RankIncrease.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            RankIncrease.ImageRect = AssetsCoordinates.Generic.Boxes.RankIncreaseBar;
            RankIncrease.SetSize((int)(dim.XScreenRatio * 1000), (int)(dim.YScreenRatio * 140));
            RankIncrease.SetPosition((int)(dim.XScreenRatio * 410), (int)(dim.YScreenRatio * 920));

            // LEVEL CURRENT POINTS BOX
            Sprite RankRadBox2 = new Sprite();
            //container.AddChild(RankRadBox2);
            RankRadBox2.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            RankRadBox2.ImageRect = AssetsCoordinates.Generic.Boxes.RankRedBox;
            RankRadBox2.SetSize((int)(dim.XScreenRatio * 140), (int)(dim.YScreenRatio * 140));
            RankRadBox2.SetPosition((int)(dim.XScreenRatio * 1470), (int)(dim.YScreenRatio * 920));

            // CHARACTER NEXT LEVEL
            Text rank2 = new Text();
            //RankRadBox2.AddChild(rank2);
            rank2.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Top);
            rank2.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(10));
            rank2.SetFont(font, dim.XScreenRatio * 20);
            rank2.Value = "LEVEL";

            // Value
            Text ranknumber2 = new Text();
            RankRadBox2.AddChild(ranknumber2);
            ranknumber2.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            ranknumber2.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(15));
            ranknumber2.SetFont(font, dim.XScreenRatio * 50);
            ranknumber2.Value = (CharacterManager.Instance.User.Level + 1).ToString();
        }

        public void UpdateLevelInfo() 
        {
            // Update races number
            _levelInfo.TotalOfPlays += 1;

            // Completed status
            _levelInfo.Completed = (int)LevelSettings.COMPLETED.TRUE;

            // Best time
            _levelInfo.BestTime = _levelInfo.BestTime == 0
                ? TrackManager.Instance.LastPlayedTrackInfo.Time
                : _levelInfo.BestTime > TrackManager.Instance.LastPlayedTrackInfo.Time
                    ? TrackManager.Instance.LastPlayedTrackInfo.Time
                    : _levelInfo.BestTime;

            // Last race Points
            _levelInfo.PointsObtained = TrackManager.Instance.LastPlayedTrackInfo.Points;

            // Total points 
            _levelInfo.TotalPoints += TrackManager.Instance.LastPlayedTrackInfo.Points;

            // Update level info
            TrackManager.Instance.SelectedTrackModel = _levelInfo;

            // Update last played level data
            var lastPlayedLevel = TrackManager.Instance.LastPlayedTrackInfo;
            lastPlayedLevel.TrackData = TrackManager.Instance.SelectedTrackModel;
            TrackManager.Instance.LastPlayedTrackInfo = lastPlayedLevel;

            // Set level data for current screen
            _postLevelData = lastPlayedLevel;

            var player = CharacterManager.Instance.User;

            // Update user experience points
            player.Experience += TrackManager.Instance.LastPlayedTrackInfo.Points;

            // Update user coins
            player.Wallet += TrackManager.Instance.LastPlayedTrackInfo.Coins;

            // Save updates
            CharacterManager.Instance.User = player;

            // Update components
            if(_postLevelData.Components > 0) {
                var collected = VehicleManager.Instance.CollectedComponents;
                foreach(var vehicle in collected.CollectedComponentsList) {
                    if(!vehicle.VehicleComponents.Brakes) {
                        vehicle.VehicleComponents.Brakes = true;
                        break;
                    }
                }
                VehicleManager.Instance.CollectedComponents = collected;
            }
        }
    }
}
