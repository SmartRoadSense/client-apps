using System;
using System.Diagnostics;
using Urho;
using Urho.Gui;
using Urho.Resources;

namespace SmartRoadSense.Shared {
    public class SceneLevelSelect : BaseScene {

        Font font;
        Sprite _container;
        UIElement root;
        ScreenInfoRatio dim; //variabile rapporto dimensioni schermo
        ResourceCache cache;
        int _counter;
        Button _prevLevel;
        Button _selLevel;
        Button _nextLevel;
        Text prevLevelN;
        Text selLevelN;
        Text nextLevelN;
        Text difficultyN;
        Text BestTimeN;
        int _racesNumber;
        int lastComplededRace;
        Window _noTrackWindow;

        public SceneLevelSelect(Game game) : base(game) {
            dim = GameInstance.ScreenInfo;
            root = GameInstance.UI.Root;
            cache = GameInstance.ResourceCache;
            font = cache.GetFont("Fonts/OpenSans-Bold.ttf");

            _racesNumber = TrackManager.Instance.TrackCount;
            if(CharacterManager.Instance.User.LastCompletedRace > 0)
                lastComplededRace = CharacterManager.Instance.User.LastCompletedRace;
            else
                lastComplededRace = 0;

            if(TrackManager.Instance.Tracks.TrackModel.Count > 1) {
                _counter = lastComplededRace + 1;
                TrackManager.Instance.SelectedTrackId = _counter;
                TrackManager.Instance.LoadSingleLevel(TrackManager.Instance.SelectedTrackId);
            }

            CreateUI();
        }

        private void CreateUI() {
            font = GameInstance.ResourceCache.GetFont(GameInstance.defaultFont);
            CreateBackground();
            CreateTopBar();
            CreateInfoText();
            CreateLevelBar();
            CreateScreenBody();
        }

        void CreateBackground() {
            //TODO: animated background
            var backgroundTexture = GameInstance.ResourceCache.GetTexture2D("Textures/MenuBackground.png");
            if(backgroundTexture == null)
                return;
            var backgroundSprite = GameInstance.UI.Root.CreateSprite();
            backgroundSprite.Texture = backgroundTexture;
            backgroundSprite.SetSize(GameInstance.ScreenInfo.SetX(ScreenInfo.DefaultScreenWidth), GameInstance.ScreenInfo.SetY(ScreenInfo.DefaultScreenHeight));
            backgroundSprite.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            backgroundSprite.SetPosition(0, 0);
        }

        void CreateTopBar() {
            var bar = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.TopBar.ResourcePath);

            var black_bar = GameInstance.UI.Root.CreateSprite();
            GameInstance.UI.Root.AddChild(black_bar);
            black_bar.Texture = bar;
            black_bar.Opacity = 0.5f;
            black_bar.SetPosition(0, GameInstance.ScreenInfo.SetY(30));
            black_bar.SetSize(GameInstance.ScreenInfo.SetX(2000), GameInstance.ScreenInfo.SetY(140));
            black_bar.ImageRect = AssetsCoordinates.Generic.TopBar.Rectangle;

            // BACK
            Button btn_back = new Button();
            GameInstance.UI.Root.AddChild(btn_back);
            btn_back.SetStyleAuto(null);
            btn_back.SetPosition(GameInstance.ScreenInfo.SetX(40), GameInstance.ScreenInfo.SetY(40));
            btn_back.SetSize(GameInstance.ScreenInfo.SetX(120), GameInstance.ScreenInfo.SetY(120));
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
            GameInstance.UI.Root.AddChild(screen_title);
            screen_title.SetStyleAuto(null);
            screen_title.SetPosition(GameInstance.ScreenInfo.SetX(1500), GameInstance.ScreenInfo.SetY(50));
            screen_title.SetSize(GameInstance.ScreenInfo.SetX(400), GameInstance.ScreenInfo.SetY(100));
            screen_title.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            screen_title.ImageRect = AssetsCoordinates.Generic.Boxes.BoxTitle;
            screen_title.Enabled = false;
            screen_title.Pressed += (PressedEventArgs args) => {
#if DEBUG
               //TrackManager.Instance.Tracks = null;
                //TrackManager.Instance.Init();
#endif
            };

            Text buttonTitleText = new Text();
            screen_title.AddChild(buttonTitleText);
            buttonTitleText.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            buttonTitleText.SetPosition(0, 0);
            buttonTitleText.SetFont(font, GameInstance.ScreenInfo.SetX(30));
            buttonTitleText.Value = "LEVEL SELECT";
        }

        void CreateInfoText() {
            var infoText = root.CreateWindow();
            infoText.SetSize(dim.SetX(1920), dim.SetY(200));
            infoText.SetPosition(dim.SetX(0), dim.SetY(160));
            infoText.SetColor(Color.Transparent);

            /*
            var text = new Text();
            text.SetFont(GameInstance.ResourceCache.GetFont(GameInstance.defaultFont), 20);
            text.SetPosition(dim.SetX(20), dim.SetY(15));
            text.Value = "Use SmartRoadSense to collect road data and unlock new levels!";
            text.UseDerivedOpacity = false;
            infoText.AddChild(text);   

                  
            var smallText = new Text();
            smallText.SetFont(GameInstance.ResourceCache.GetFont(GameInstance.defaultFont), 8);
            smallText.SetPosition(dim.SetX(20), dim.SetY(67));
            smallText.Value = "*This feature will be available starting from the next app release. Stay tuned for updates!";
            smallText.UseDerivedOpacity = false;
            infoText.AddChild(smallText);
            */           
        }

        void CreateLevelBar() {
            _container = root.CreateSprite();
            _container.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            _container.ImageRect = AssetsCoordinates.Generic.Boxes.ContainerTrasparent;
            _container.SetSize((int)(dim.XScreenRatio * 800), (int)(dim.YScreenRatio * 400));
            _container.SetPosition(GameInstance.ScreenInfo.SetX(500), GameInstance.ScreenInfo.SetY(30));

            Sprite LevelToComplete = new Sprite();
            _container.AddChild(LevelToComplete);
            LevelToComplete.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            LevelToComplete.ImageRect = AssetsCoordinates.Generic.Boxes.LevelToComplete;
            LevelToComplete.SetSize((int)(dim.XScreenRatio * 400), (int)(dim.YScreenRatio * 100));
            LevelToComplete.SetPosition(GameInstance.ScreenInfo.SetX(350), GameInstance.ScreenInfo.SetY(240));

            //per comodità utilizzo direttamente IntRect e non AssetsCoordinate
            // in questo punto dovrebbe essere creato l'IntRect utilizzando la percentuale di completamento del livello
            var buttons = cache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            double totPoints = CharacterLevelData.PointsToNextLevel();
            double currPoints = CharacterLevelData.CurrentLevelPoints();
            double currX = currPoints / totPoints * 100;
            double totX = dim.SetX(400);

            int x = 2017; // punto d'inizio "left" dell'immagine del bottone
            int y = (int)Math.Round(x + currX / 100 * totX);
            int size = y - x;

            Sprite LevelCompleted = new Sprite();
            _container.AddChild(LevelCompleted);
            LevelCompleted.Texture = buttons;
            LevelCompleted.ImageRect = new IntRect(x, 1410, y, 1471);
            LevelCompleted.SetSize(dim.SetX(size), dim.SetY(100));
            LevelCompleted.SetPosition(dim.SetX(350), dim.SetY(240));
            //LevelCompleted.SetPosition((int)(dim.XScreenRatio * 220), (int)(dim.YScreenRatio * 220));

            // LEVEL INDICATOR
            Sprite LevelInd = new Sprite();
            _container.AddChild(LevelInd);
            LevelInd.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            LevelInd.ImageRect = AssetsCoordinates.Generic.Boxes.LevelBlueBox;
            LevelInd.SetSize((int)(dim.XScreenRatio * 140), (int)(dim.YScreenRatio * 140));
            LevelInd.SetPosition(GameInstance.ScreenInfo.SetX(220), GameInstance.ScreenInfo.SetY(220));

            // Title
            Text level = new Text();
            LevelInd.AddChild(level);
            level.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Top);
            level.SetPosition(0, GameInstance.ScreenInfo.SetY(10));
            level.SetFont(font, dim.XScreenRatio * 20);
            level.Value = "LEVEL";

            // Value
            Text levelnumber = new Text();
            LevelInd.AddChild(levelnumber);
            levelnumber.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            levelnumber.SetPosition(0, GameInstance.ScreenInfo.SetY(20));
            levelnumber.SetFont(font, dim.XScreenRatio * 50);
            levelnumber.Value = string.Format("{0}", CharacterManager.Instance.User.Level);
        }

        void CreateScreenBody() {
            _container = root.CreateSprite();
            _container.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            _container.ImageRect = AssetsCoordinates.Generic.Boxes.ContainerTrasparent;
            _container.SetSize((int)(dim.XScreenRatio * 1200), (int)(dim.YScreenRatio * 1400));
            _container.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(240));

            // No track window
            _noTrackWindow = new Window();
            _container.AddChild(_noTrackWindow);
            _noTrackWindow.SetPosition(GameInstance.ScreenInfo.SetY(1180), GameInstance.ScreenInfo.SetY(200));
            _noTrackWindow.SetSize(GameInstance.ScreenInfo.SetX(650), GameInstance.ScreenInfo.SetY(450));
            _noTrackWindow.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            _noTrackWindow.ImageRect = AssetsCoordinates.Generic.Boxes.PauseMenuBox;
            _noTrackWindow.Opacity = 0.5f;
            //_noTrackWindow.SetColor(Color.Black);

            var noTrackText = new Text();
            _noTrackWindow.AddChild(noTrackText);
            noTrackText.SetPosition(GameInstance.ScreenInfo.SetX(20), GameInstance.ScreenInfo.SetY(15));
            noTrackText.SetSize((int)(dim.XScreenRatio * 610), (int)(dim.YScreenRatio * 370));
            noTrackText.UseDerivedOpacity = false;
            noTrackText.Value = "Use SmartRoadSense to collect at least 15 minutes of road data and unlock new levels!";
            noTrackText.Wordwrap = true;
            noTrackText.SetColor(Color.White);
            noTrackText.SetFont(font, 20);

            /*
            ScrollBar scrollBar = new ScrollBar();
            container.AddChild(scrollBar);

            scrollBar.SetPosition(GameInstance.ScreenInfo.SetX(10), GameInstance.ScreenInfo.SetY(10));
            scrollBar.SetSize(GameInstance.ScreenInfo.SetX(1800), GameInstance.ScreenInfo.SetY(20));
            scrollBar.SetColor(Color.Cyan);
            scrollBar.Range = TrackManager.Instance.TrackCount;

            var slider = scrollBar.Slider;
            slider.Range = TrackManager.Instance.TrackCount;
            slider.SetColor(Color.Blue);
           
             slider.DragEnd += (DragEndEventArgs args) => {
                Debug.WriteLine("Scrollbar Value: " + scrollBar.Value);
                Debug.WriteLine("Slider: " + slider.Value);
            };
            */
            _prevLevel = new Button();
            _container.AddChild(_prevLevel);
            _prevLevel.SetStyleAuto(null);
            _prevLevel.Opacity = 0.5f;
            _prevLevel.SetPosition(GameInstance.ScreenInfo.SetX(250), GameInstance.ScreenInfo.SetY(250));
            _prevLevel.SetSize((int)(dim.YScreenRatio * 400), (int)(dim.YScreenRatio * 400));
            _prevLevel.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            //prev_level.ImageRect = AssetsCoordinates.Generic.Boxes.LevelBeach;
            _prevLevel.Pressed += (PressedEventArgs args) => {
                PreviousRace();
             };

            Text prevLevel = new Text();
            _prevLevel.AddChild(prevLevel);
            prevLevel.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            prevLevel.SetPosition(0, GameInstance.ScreenInfo.SetY(30));
            prevLevel.Opacity = 0.5f;
            prevLevel.SetFont(font, dim.XScreenRatio * 30);
            prevLevel.SetColor(Color.Black);
            prevLevel.Value = "TRACK";

            prevLevelN = new Text();
            _prevLevel.AddChild(prevLevelN);
            prevLevelN.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            prevLevelN.SetPosition(0, GameInstance.ScreenInfo.SetY(120));
            prevLevelN.Opacity = 0.5f;
            prevLevelN.SetFont(font, dim.XScreenRatio * 70);
            prevLevelN.SetColor(Color.Black);
            prevLevelN.Value = "1";

            /* SELECTED */
            _selLevel = new Button();
            _container.AddChild(_selLevel);
            _selLevel.SetStyleAuto(null);
            _selLevel.SetPosition(GameInstance.ScreenInfo.SetY(700), GameInstance.ScreenInfo.SetY(200));
            _selLevel.SetSize((int)(dim.XScreenRatio * 450), (int)(dim.YScreenRatio * 450));
            _selLevel.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);

            Text selLevel = new Text();
            _selLevel.AddChild(selLevel);
            selLevel.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            selLevel.SetPosition(GameInstance.ScreenInfo.SetX(-40), GameInstance.ScreenInfo.SetY(40));
            selLevel.SetFont(font, dim.XScreenRatio * 50);
            selLevel.SetColor(Color.Black);
            selLevel.Value = "TRACK";

            selLevelN = new Text();
            _selLevel.AddChild(selLevelN);
            selLevelN.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            selLevelN.SetPosition(GameInstance.ScreenInfo.SetX(100), GameInstance.ScreenInfo.SetY(40));
            selLevelN.SetFont(font, dim.XScreenRatio * 50);
            selLevelN.SetColor(Color.Black);
            selLevelN.Value = "";

            Text difficulty = new Text();
            _selLevel.AddChild(difficulty);
            difficulty.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Center);
            difficulty.SetPosition(GameInstance.ScreenInfo.SetX(20), GameInstance.ScreenInfo.SetY(120));
            difficulty.SetFont(font, dim.XScreenRatio * 30);
            difficulty.SetColor(Color.Black);
            difficulty.Value = "Difficulty:";

            difficultyN = new Text();
            _selLevel.AddChild(difficultyN);
            difficultyN.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Center);
            difficultyN.SetPosition(GameInstance.ScreenInfo.SetX(-20), GameInstance.ScreenInfo.SetY(120));
            difficultyN.SetFont(font, dim.XScreenRatio * 30);
            difficultyN.SetColor(Color.Gray);
            difficultyN.Value = "N/A";

            Text BestTime = new Text();
            _selLevel.AddChild(BestTime);
            BestTime.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Center);
            BestTime.SetPosition(GameInstance.ScreenInfo.SetX(20), GameInstance.ScreenInfo.SetY(180));
            BestTime.SetFont(font, dim.XScreenRatio * 30);
            BestTime.SetColor(Color.Black);
            BestTime.Value = "Best Time:";

            BestTimeN = new Text();
            _selLevel.AddChild(BestTimeN);
            BestTimeN.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Center);
            BestTimeN.SetPosition(GameInstance.ScreenInfo.SetX(-20), GameInstance.ScreenInfo.SetY(180));
            BestTimeN.SetFont(font, dim.XScreenRatio * 30);
            BestTimeN.SetColor(Color.Gray);
            BestTimeN.Value = "00:00:0";

            /*END SELECTED */

            _nextLevel = new Button();
            _container.AddChild(_nextLevel);
            _nextLevel.Opacity = 0.5f;
            _nextLevel.SetStyleAuto(null);
            _nextLevel.SetPosition(GameInstance.ScreenInfo.SetX(1280), GameInstance.ScreenInfo.SetY(250));
            _nextLevel.SetSize((int)(dim.XScreenRatio * 400), (int)(dim.YScreenRatio * 400));
            _nextLevel.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            _nextLevel.Pressed += (PressedEventArgs args) => {
                NextRace();
            };

            Text nextLevel = new Text();
            _nextLevel.AddChild(nextLevel);
            nextLevel.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            nextLevel.SetPosition(0, GameInstance.ScreenInfo.SetY(30));
            nextLevel.Opacity = 0.5f;
            nextLevel.SetFont(font, dim.XScreenRatio * 30);
            nextLevel.SetColor(Color.Black);
            nextLevel.Value = "TRACK";

            nextLevelN = new Text();
            _nextLevel.AddChild(nextLevelN);
            nextLevelN.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            nextLevelN.SetPosition(0, GameInstance.ScreenInfo.SetY(120));
            nextLevelN.Opacity = 0.5f;
            nextLevelN.SetFont(font, dim.XScreenRatio * 70);
            nextLevelN.SetColor(Color.Black);
            nextLevelN.Value = "3";

            UpdateActiveLevels();

            /*START BUTTONS - random & selected level*/

            Button StartRandom = new Button();
            _container.AddChild(StartRandom);
            StartRandom.SetStyleAuto(null);
            //StartRandom.Opacity = 0.25f;
            StartRandom.SetPosition(GameInstance.ScreenInfo.SetX(100), GameInstance.ScreenInfo.SetY(700));
            StartRandom.SetSize((int)(dim.XScreenRatio * 700), (int)(dim.YScreenRatio * 100));
            StartRandom.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            StartRandom.ImageRect = AssetsCoordinates.Generic.Boxes.RankIncreaseBar;
            StartRandom.Pressed += (PressedEventArgs args) => {
                //SessionManager.Instance.SelectedLevelId = "smoothed-averaged-20.csv";
                TrackManager.Instance.SelectedTrackId = 0;
                Launcher(true);
            };

            Text level = new Text();
            StartRandom.AddChild(level);
            level.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            level.SetPosition(0, 0);
            level.SetFont(font, dim.SetX(40));
            level.SetColor(Color.White);
            level.Value = "RANDOM TRACK";

            Button StartGame = new Button();
            _container.AddChild(StartGame);
            StartGame.SetStyleAuto(null);
            StartGame.SetPosition(GameInstance.ScreenInfo.SetX(1150), GameInstance.ScreenInfo.SetY(700));
            StartGame.SetSize((int)(dim.XScreenRatio * 700), (int)(dim.YScreenRatio * 100));
            StartGame.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            StartGame.ImageRect = AssetsCoordinates.Generic.Boxes.RaceCompleted;
            StartGame.Pressed += (PressedEventArgs args) => {
                if(_counter >= 1)
                    Launcher();
            };

            Text go = new Text();
            StartGame.AddChild(go);
            go.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            go.SetPosition(0, 0);
            go.SetFont(font, dim.SetX(40));
            go.SetColor(Color.White);
            go.Value = "GO!";

            /* END START BUTTONS */
        }

        void UpdateActiveLevels() {
            if(_racesNumber == 1) {
                // Only test track available
                _prevLevel.Visible = false;
                _selLevel.Enabled = false;
                _selLevel.ImageRect = AssetsCoordinates.Generic.Boxes.LevelBlocked;
                _nextLevel.Enabled = false;
                _nextLevel.Visible = false;
                difficultyN.Value = "0";
                BestTimeN.Value = "N/A";

                _noTrackWindow.Visible = true;
            }
            else {
                if(_counter == 1) { // DON'T SHOW LEVEL 0 --> RANDOM LEVEL
                    _noTrackWindow.Visible = true;
                    _prevLevel.Visible = false;
                    _selLevel.Enabled = false;
                    _selLevel.ImageRect = AssetsCoordinates.Generic.Boxes.LevelBlocked;
                    _nextLevel.Enabled = true;
                    _nextLevel.ImageRect = AssetsCoordinates.Generic.Boxes.LevelBlocked;
                }
                else {
                    _noTrackWindow.Visible = false;

                    var prevLevel = TrackManager.Instance.LoadSingleLevel(_counter - 1);
                    _prevLevel.Visible = true;
                    _prevLevel.Enabled = true;
                    _prevLevel.ImageRect = SelectLevelLandskape(prevLevel);
                    prevLevelN.Value = string.Format("{0}", prevLevel.IdTrack);
                }

                var level = TrackManager.Instance.LoadSingleLevel(_counter);
                _selLevel.Enabled = true;
                _selLevel.Visible = true;
                _selLevel.ImageRect = SelectLevelLandskape(level);
                selLevelN.Value = "" + level.IdTrack;
                difficultyN.Value = "" + level.Difficulty;
                BestTimeN.Value = string.Format("{0}", TimeSpan.FromMilliseconds(TrackManager.Instance.SelectedTrackModel.BestTime).MillisRepresentation());

                if(_counter + 1 >= _racesNumber) {
                    _noTrackWindow.Visible = true;
                    _nextLevel.Enabled = false;
                    _nextLevel.Visible = false;
                }
                else {
                    _noTrackWindow.Visible = false;

                    var nextLevel = TrackManager.Instance.LoadSingleLevel(_counter + 1);
                    _nextLevel.Enabled = true;
                    _nextLevel.Visible = true;
                    _nextLevel.ImageRect = SelectLevelLandskape(nextLevel);
                    nextLevelN.Value = string.Format("{0}", nextLevel.IdTrack);
                }
            }
        }

        IntRect SelectLevelLandskape(TrackModel level) {
            var land = AssetsCoordinates.Generic.Boxes.LevelBlocked;
            int landskape = level.Landskape;
            int completed = level.Completed;
            // TODO: set blocked levels
            //if (completed == 0 && counter > lastComplededRace + 1 && !(counter == lastComplededRace + 1)) {
            //    land = AssetsCoordinates.Generic.Boxes.LevelBlocked;
            //}
            //else {
                if(landskape == 0) {
                    Random rnd = new Random();
                    landskape = rnd.Next(1, 4); // creates a number between 1 and 3                
                }

                switch(landskape) {
                    case 1:
                        land = AssetsCoordinates.Generic.Boxes.LevelSnow;
                        break;
                    case 2:
                        land = AssetsCoordinates.Generic.Boxes.LevelMoon;
                        break;
                    case 3:
                        land = AssetsCoordinates.Generic.Boxes.LevelBeach;
                        break;
                }
            //}
            
            return land;
        }

        void NextRace() {
            if(_counter >= 0 && _counter <= _racesNumber) {
                if(_counter == _racesNumber) {
                    _counter = _racesNumber;
                }
                else {
                    _counter = 1 + _counter;
                }
                TrackManager.Instance.SelectedTrackId = _counter;
                UpdateActiveLevels();
            }
        }

        void PreviousRace() {
            if(_counter > 0 && _counter <= _racesNumber) {
                _counter = _counter <= 0 ? _counter : _counter - 1;
                TrackManager.Instance.SelectedTrackId = _counter;
                UpdateActiveLevels();
            }
        }

        void Launcher(bool testLevel = false) {
            if(testLevel)
                GameInstance.LaunchScene(GameScenesEnumeration.GAME, true);
            else
                GameInstance.LaunchScene(GameScenesEnumeration.GAME);
        }
    }

}
    

       

