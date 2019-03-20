using System;
using System.Diagnostics;
using Urho;
using Urho.Gui;
using Urho.Urho2D;
using Urho.Resources;
using System.Collections.Generic;
using System.Linq;
using Urho.Audio;

namespace SmartRoadSense.Shared
{
    public class CsvRecord
    {
        public float Ppe { get; set; }
    }

    public enum GameState
    {
        PLAYING = 0,
        PAUSE = 1,
        GAME_OVER = 2,
        LEVEL_COMPLETE = 3
    }

    public class SceneGame : BaseScene
    {
        const uint NumObjects = 1;
        const float CameraDistance = 2.5f;
        const float _cameraStartingX = 0.0f;
        const float _cameraStartingY = 0.0f;
        const float _cameraStartingZ = -2.0f;
        const float _vehicleOffsetX = 1.5f;
        const float _vehicleOffsetY = 1.2f;
        const float _speedStep = (float)Math.PI / 2;

        const string _balanceBodyName = "BalanceBody";
        const string _groundBodyName = "GroundBody";
        const string _groundColorNodeName = "GroundColorNode";
        const string _finishLineName = "FinishLineNode";
        const string _coinCollisionName = "CoinNode";
        const string _componentCollisionName = "ComponentNode";

        const string _soundVolumeSlider = "SFX";
        const string _musicVolumeSlider = "MUSIC";

        Vehicle _vehicle;
        GameState _gameState;

        Node _balanceObject;
        Node CameraNode { get; set; }
        Node GroundColorNode { get; set; }
        Action<PostUpdateEventArgs> PostUpdate { get; set; }
        Action<PostRenderUpdateEventArgs> PostRenderUpdate { get; set; }
        PhysicsWorld2D _physicsWorld2D;
        CollisionChain2D collisionChain;
        List<TerrainGenerator.Point> terrainData = new List<TerrainGenerator.Point>();

        Button btnA;
        Button btnB;
        Button btnL;
        Button btnR;
        Button btnBoost;

        bool _finishLinePassed { get; set; }
        bool TouchEnabled { get; set; }
        const float TouchSensitivity = 2;
        int _lastBackground;
        float _finishLineEndX;
        bool _timer;
        readonly bool _randomLevel;
        int _coins;
        int _coinPositionedCounter;
        int _components;
        bool _componentsCollected;
        bool _removedFirstCoin;

        Stopwatch _stopwatch;
        GameDistanceData _distanceData;
        MapPositionData _mapPositionData;

        static readonly Random random = new Random();
        /// Return a random float between 0.0 (inclusive) and 1.0 (exclusive.)
        public static float NextRandom() { return (float)random.NextDouble(); }
        /// Return a random float between 0.0 and range, inclusive from both ends.
        public static float NextRandom(float range) { return (float)random.NextDouble() * range; }
        /// Return a random float between min and max, inclusive from both ends.
        public static float NextRandom(float min, float max) { return (float)((random.NextDouble() * (max - min)) + min); }
        /// Return a random integer between min and max - 1.
        public static int NextRandom(int min, int max) { return random.Next(min, max); }

        // Scenery data
        List<Node> FgList = new List<Node>();
        List<Node> Bg1List = new List<Node>();
        List<Node> Bg2List = new List<Node>();
        List<Node> Bg3List = new List<Node>();

        // Level data
        readonly TrackModel _levelData;
        readonly int _lvlLandscape;
        Action<EndViewRenderEventArgs> _actionCloseSplashScreen;

        public SceneGame(Game game, bool randomLevel) : base(game) {

            // Reset scene
            GameInstance.ResourceCache.ReleaseAllResources();
            RemoveAllComponents();
            RemoveAllChildren();

            // Get level data
            _levelData = TrackManager.Instance.SelectedTrackModel;

            // Init scene
            _randomLevel = randomLevel;

            if(_randomLevel)
                _lvlLandscape = random.Next(1, 4);
            else
                _lvlLandscape = _levelData.Landskape;

            // Start music and splashscreen
            InitSounds();
            SplashScreen();

            CreateBackgroundScene();
            CreateForegroundScene();
            CreateOSD();

            // Init On-Screen Joystick
            InitOSD();

            // Init viewport
            SetupViewport();

            SubscribeToEvents();

            // Notify main loop of game
            GameInstance.IsRunningGame = true;
            GameInstance.GamePaused = false;
            GameInstance.CameraNode = CameraNode;
            GameInstance.GameVehicle = _vehicle;
            GameInstance.UI.SetFocusElement(null);

            _gameState = GameState.PLAYING;
            _timer = false;

#if DEBUG
            //GameInstance.InitGameDebugWindow();
            //GameInstance.DrawDebugControls();
#endif
        }

        void InitSounds() {
            // First start sound
            if(GetChild("Music", false) != null) { }
            else {
                // INIT MUSIC 
                // Set saved music level
                GameInstance.Audio.SetMasterGain(SoundType.Music.ToString(), SoundManager.Instance.MusicGain);
                GameInstance.Audio.SetMasterGain(SoundType.Effect.ToString(), SoundManager.Instance.EffectsGain);

                Sound music;
                switch(_lvlLandscape) {
                    case 1:
                        music = GameInstance.ResourceCache.GetSound(SoundLibrary.Music.Beach);
                        break;
                    case 2:
                        music = GameInstance.ResourceCache.GetSound(SoundLibrary.Music.Moon);
                        break;
                    case 3:
                        music = GameInstance.ResourceCache.GetSound(SoundLibrary.Music.SnowyForest);
                        break;
                    default:
                        music = GameInstance.ResourceCache.GetSound(SoundLibrary.Music.Moon);
                        break;
                }
                music.Looped = true;
                Node musicNode = CreateChild("Music");
                SoundSource musicSource = musicNode.CreateComponent<SoundSource>();

                // Set the sound type to music so that master volume control works correctly
                musicSource.SetSoundType(SoundType.Music.ToString());
                musicSource.Play(music);
                // End music code
            }
        }

        void SplashScreen() 
        {
            var splashScrName = SplashScreenCreator.CreateSplashScreen(GameInstance, this);
            GameInstance.Engine.RunFrame();

            _actionCloseSplashScreen = (EndViewRenderEventArgs args) => {
                var splashUI = GameInstance.UI.Root.GetChild(splashScrName);
                if(splashUI == null)
                    return;

                var container = splashUI.GetChild("splashScreenTextWindow") as Window;
                var btn = container.GetChild("ButtonContinue") as Button;
                btn.Visible = true;
                btn.Pressed += (PressedEventArgs arg) => {
                    splashUI.Visible = false;
                    GameInstance.Renderer.EndViewRender -= _actionCloseSplashScreen;
                };
            };
        }

        void SetupViewport()
        {
            var renderer = GameInstance.Renderer;
            renderer.SetViewport(0, new Viewport(Context, this, CameraNode.GetComponent<Camera>(), null));

            // HD UI Settings
            GameInstance.Renderer.GetViewport(0).RenderPath.Append(CoreAssets.PostProcess.FXAA2);

            _physicsWorld2D.SubStepping = false;
        }

        void SubscribeToEvents()
        {
            GameInstance.Renderer.EndViewRender += _actionCloseSplashScreen;

            PostUpdate = new Action<PostUpdateEventArgs>((PostUpdateEventArgs obj) => {

                if(_vehicle.MainBody == null)
                    return;

                // Stop camera if vehicle reached left border area
                if(CameraNode.Position.X < _cameraStartingX && _vehicle.MainBody.LinearVelocity.X < 0) { return; }
                if(_vehicle.MainBody.Node.Position.X + _vehicleOffsetX < _cameraStartingX && _vehicle.MainBody.LinearVelocity.X > 0) { return; }

                // Move camera with vehicle
                CameraNode.Position = new Vector3(_vehicle.MainBody.Node.Position.X + _vehicleOffsetX, _vehicle.MainBody.Node.Position.Y + _vehicleOffsetY, _cameraStartingZ);

                // Update game time
                if(_stopwatch != null) {
                    var currTimeBox = GameInstance.UI.Root.GetChild("timeBox");
                    Text currTimeText = (Text)currTimeBox.GetChild("currentTimeText");
                    currTimeText.Value = _stopwatch.GetElapsedTime().MillisRepresentation();
                }

                // Update distance travelled
                var distanceBox = GameInstance.UI.Root.GetChild("distanceBox");
                Text distanceText = (Text)distanceBox.GetChild("distanceText");
                distanceText.Value = _distanceData.DistanceText(_vehicle.MainBody.Node.Position.X);

                // Update map indicator
                var mapBox = GameInstance.UI.Root.GetChild("mapBox");
                var MapIndicator = mapBox.GetChild("mapBar");
                MapIndicator.SetPosition(_mapPositionData.LocatorPosition(_vehicle.MainBody.Node.Position.X), MapIndicator.Position.Y);

                // Check if finish line has been reached
                if(!_finishLinePassed && _vehicle.MainBody.Node.Position.X >= _finishLineEndX)
                {
                    if(_gameState == GameState.PLAYING) {
                        Debug.WriteLine("PASSED FINISH LINE");
                        _finishLinePassed = true;
                        _stopwatch.Pause();

                        LevelComplete();
                    }
                }
            });

            PostRenderUpdate = new Action<PostRenderUpdateEventArgs>((PostRenderUpdateEventArgs obj) => {
                var debugRendererComp = GetComponent<DebugRenderer>();
                var physicsComponent = GetComponent<PhysicsWorld2D>();
                if(physicsComponent != null && GameInstance.ShowCollisionGeometry) {
                    physicsComponent.DrawDebugGeometry(debugRendererComp, false);
                }
            });

            GameInstance.Engine.PostUpdate += PostUpdate;
            GameInstance.Engine.PostRenderUpdate += PostRenderUpdate;

            GameInstance.PostUpdate = PostUpdate;
            GameInstance.PostRenderUpdate = PostRenderUpdate;
        }

        void CreateBackgroundScene()
        {
            CreateComponent<Octree>();
            CreateComponent<DebugRenderer>();

            // Create 2D physics world component
            _physicsWorld2D = CreateComponent<PhysicsWorld2D>();
            _physicsWorld2D.PhysicsBeginContact2D += (PhysicsBeginContact2DEventArgs obj) => {

                // Coin collect
                if(string.Equals(obj.BodyA.Node.Name, _coinCollisionName) && !string.Equals(obj.BodyB.Node.Name, _coinCollisionName)) {
                    obj.NodeA.Remove();
                    _coins += 1;
                    // Update coin position
                    var coinsBox = GameInstance.UI.Root.GetChild("coinsBox");
                    var coinsText = (Text)coinsBox.GetChild("coinsText");
                    coinsText.Value = string.Format("{0}", _coins);
                    // Play sound
                    Sound sound = GameInstance.ResourceCache.GetSound(SoundLibrary.SFX.Coin);
                    if(sound != null) {
                        // Create a scene node with a SoundSource component for playing the sound. The SoundSource component plays
                        // non-positional audio, so its 3D position in the scene does not matter. For positional sounds the
                        // SoundSource3D component would be used instead
                        Node soundNode = CreateChild("SoundCoin");
                        SoundSource soundSource = soundNode.CreateComponent<SoundSource>();
                        soundSource.SetSoundType(SoundType.Effect.ToString());
                        soundSource.Play(sound);
                        // In case we also play music, set the sound volume below maximum so that we don't clip the output
                        soundSource.Gain = 0.75f;
                        // Set the sound component to automatically remove its scene node from the scene when the sound is done playing
                    }
                }

                // Component collect
                if(string.Equals(obj.BodyA.Node.Name, _componentCollisionName) && !string.Equals(obj.BodyB.Node.Name, _componentCollisionName)) {
                    obj.NodeA.Remove();
                    _components += 1;
                    // Update coin position
                    var componentsBox = GameInstance.UI.Root.GetChild("componentsBox");
                    var componentsText = (Text)componentsBox.GetChild("componentsText");
                    componentsText.Value = string.Format("{0}", _components);
                    // Play sound
                    Sound sound = GameInstance.ResourceCache.GetSound(SoundLibrary.SFX.Component);
                    if(sound != null) {
                        Node soundNode = CreateChild("SoundComponent");
                        SoundSource soundSource = soundNode.CreateComponent<SoundSource>();
                        soundSource.SetSoundType(SoundType.Effect.ToString());
                        soundSource.Play(sound);
                        soundSource.Gain = 0.75f;
                    }
                }

                // Game over
                if(string.Equals(obj.BodyA.Node.Name, _balanceBodyName) && string.Equals(obj.BodyB.Node.Name, _groundBodyName)
                   || string.Equals(obj.BodyA.Node.Name, _groundBodyName) && string.Equals(obj.BodyB.Node.Name, _balanceBodyName))
                {
                    if(_gameState == GameState.PLAYING && !GameInstance.ShowCollisionGeometry)
                    GameOver();
                }
            };

            // Create camera node
            CameraNode = CreateChild("Camera");
            // Set camera's position
            CameraNode.Position = (new Vector3(_cameraStartingX, _cameraStartingY, _cameraStartingZ));

            Camera camera = CameraNode.CreateComponent<Camera>();
            camera.Orthographic = true;

            camera.OrthoSize = GameInstance.Graphics.Width * Application.PixelSize;
            camera.Zoom = CameraDistance * Math.Min(GameInstance.ScreenInfo.XScreenRatio, GameInstance.ScreenInfo.YScreenRatio);

            // Set default background
            var zone = CameraNode.CreateComponent<Zone>();
            zone.FogColor = BackgroundFogColor();

            // Create node for ground color
            GroundColorNode = CreateChild(_groundColorNodeName, CreateMode.Local);
            GroundColorNode.Position = new Vector3(0.0f, -1.5f, 0.0f);

            // Create ground.
            Node groundNode = CreateChild(_groundBodyName);
            groundNode.Position = (new Vector3(0.0f, -1.5f, 0.0f));

            groundNode.NodeCollision += (NodeCollisionEventArgs obj) => {
                Debug.WriteLine("node: {0} othernode: {1}", obj.Body.Node.Name, obj.OtherBody.Node.Name);
            };

            // Create 2D rigid body for gound
            IEnumerable<float> recs = new List<float>();

            if(_randomLevel) 
            {
                var difficulty = NextRandom(5, 101);
                recs = Smoothing.SmoothTrack(Smoothing.TestPpeTrack(), difficulty);
                //recs = TerrainGenerator.RandomTerrain();
                terrainData = TerrainGenerator.ArrayToMatrix(recs.ToList());
            }
            else 
            {
                // TODO: get data based on selected level
                var lvlId = TrackManager.Instance.SelectedTrackId;
                recs = Smoothing.SmoothTrack(Smoothing.TestPpeTrack(), CharacterManager.Instance.User.Level);
                terrainData = TerrainGenerator.ArrayToMatrix(recs.ToList());
            }

            collisionChain = groundNode.CreateComponent<CollisionChain2D>();
            collisionChain.VertexCount = (uint)terrainData.Count;

            foreach (var point in terrainData)
                collisionChain.SetVertex(point.Vertex, point.Vector);

            collisionChain.Friction = 1.0f; // Set friction - needs friction for wheels to turn
            collisionChain.Restitution = 0.0f; // Set restitution (0.0 = no bounce)

            var groundBody = groundNode.CreateComponent<RigidBody2D>();
            groundBody.AddCollisionShape2D(collisionChain);

            // DRAW GROUND TERRAIN

            for(var i = 0; i < recs.Count(); i++) {
                if(i > 0) {
                    var vertex1 = collisionChain.GetVertex((uint)i - 1);
                    var vertex2 = collisionChain.GetVertex((uint)i);
                    DrawTerrainFill(vertex1.X, vertex1.Y, vertex2.X, vertex2.Y, i);
                    if(i % 2 == 0 && vertex1.X > 2)
                        PlaceCoin(vertex1);
                }
            }

            // DRAW BACKGROUND IMAGES
            // TMP SPRITE NEEDED FOR SPRITE PIXEL WIDTH
            var bgPath = AssetsCoordinates.Backgrounds.Moon.Background3.Path + AssetsCoordinates.Backgrounds.GameplayBackgroundsGeneric.Background3.Prefix + "1" + AssetsCoordinates.Backgrounds.GameplayBackgroundsGeneric.Background3.Suffix;
            var tmpSprite = GameInstance.ResourceCache.GetSprite2D(bgPath);
            tmpSprite.Rectangle = AssetsCoordinates.Backgrounds.GameplayBackgroundsGeneric.Background3.ImageRect;
            //
            var backgroundsToDraw = recs.Count() * TerrainGenerator.TerrainStepLength / (tmpSprite.Rectangle.Right * Application.PixelSize);

            for(var i = 0; i < (int)backgroundsToDraw; i++) {
                DrawBackground(i);
            }
            GameInstance.FgNode = FgList;
            GameInstance.Bg1Node = Bg1List;
            GameInstance.Bg2Node = Bg2List;
            GameInstance.Bg3Node = Bg3List;

            // SET STARTING BOUNDARY
            CollisionEdge2D startBoundary = groundNode.CreateComponent<CollisionEdge2D>();
            var x = _cameraStartingX - Application.PixelSize * tmpSprite.Rectangle.Right / 2;
            startBoundary.Vertex1 = new Vector2(x, 0.0f);
            startBoundary.Vertex2 = new Vector2(x, 10.0f);
            startBoundary.Friction = 1.0f; // Set friction - needs friction for wheels to turn
            startBoundary.Restitution = 0.5f; // Set restitution (0.0 = no bounce)

            // SET FINISH LINE
            // node object
            _finishLineEndX = TerrainGenerator.TerrainBeginningOffset + TerrainGenerator.TerrainStepLength * TerrainGenerator.TerrainEndPoints;
            //_finishLineEndX = TerrainGenerator.TerrainBeginningOffset + 30.0f;

            var finishLineNode = CreateChild(_finishLineName);
            finishLineNode.Position = new Vector3(_finishLineEndX, 0.0f, 0.0f);
            var finishLineBody = finishLineNode.CreateComponent<RigidBody2D>();

            // sprite texture
            var finishLineSprite = GameInstance.ResourceCache.GetSprite2D(AssetsCoordinates.Level.FinishLine.ResourcePath);
            finishLineSprite.Rectangle = AssetsCoordinates.Level.FinishLine.Rectangle;

            // sprite object
            StaticSprite2D finishLineStaticSprite = finishLineNode.CreateComponent<StaticSprite2D>();
            finishLineStaticSprite.Sprite = finishLineSprite;

            tmpSprite = null;

            // INIT MAP BOX & DISTANCE TRAVELLED
            _distanceData = new GameDistanceData(0/* TODO change if vehicle starting position changes */, _finishLineEndX);
            _mapPositionData = new MapPositionData(0, _finishLineEndX);
        }

        void CreateForegroundScene() {
            // ADD VEHICLE
            var vehicleMain = new VehicleCreator(this, GameInstance.ResourceCache);

            // TODO: pass on selected balance object
            _vehicle = vehicleMain.InitCarInScene(VehicleLoad.BOX);

            // Create balance object
            _balanceObject = BalanceObjectCreator.CreateBalanceObject(GameInstance, this, _balanceBodyName);
            _vehicle.BalanceObject = _balanceObject;
        }

        void CreateOSD()
        {
            var pauseButton = GameButton.CreateButton(GameInstance.UI.Root, GameInstance.ScreenInfo, 20, 20, 120, 120, HorizontalAlignment.Left, VerticalAlignment.Top);
            pauseButton.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            pauseButton.ImageRect = AssetsCoordinates.Generic.Icons.PauseButton;

            pauseButton.Pressed += (PressedEventArgs args) => {
                Debug.WriteLine("Pause button pressed");
                PauseMenu();
            };
        }

        void ScaleToScreenSize(IReadOnlyList<UIElement> children, int level = 0)
        {
            foreach (var control in children)
            {
                Debug.WriteLine("level " + level + ": " + control.Name + " - " + control.TypeName);
                if(control.TypeName.Equals("Button") || control.TypeName.Equals("BorderImage"))
                {
                    control.SetSize(GameInstance.ScreenInfo.SetX(control.Size.X), GameInstance.ScreenInfo.SetX(control.Size.Y));    
                    control.SetPosition(GameInstance.ScreenInfo.SetX(control.Position.X), GameInstance.ScreenInfo.SetX(control.Position.Y));    
                }

                if(control.Children.Count > 0)
                    ScaleToScreenSize(control.Children, level++);
            }
        }

        void DrawTerrainFill(float x1, float y1, float x2, float y2, int idx)
        {
            var geom = GroundColorNode.CreateComponent<CustomGeometry>(CreateMode.Local, (uint)idx);
            geom.BeginGeometry(0, PrimitiveType.TriangleStrip);

            var material = new Material();
            material.SetTechnique(0, CoreAssets.Techniques.NoTextureUnlitVCol);
            geom.SetMaterial(material);

            var terrainColor = new Color(26.0f / 255.0f, 26.0f / 255.0f, 26.0f / 255.0f, 1.0f);
            var terrainEndColor = new Color(130.0f / 255.0f, 120.0f / 255.0f, 120.0f / 255.0f, 1.0f);

            var A = new Vector3(x1, y1, 0.0f);          /* A-B */
            var B = new Vector3(x2, y2, 0.0f);          /* | | */
            var C = new Vector3(x2, y2 -6.0f, 0.0f);    /* | | */
            var D = new Vector3(x1, y1 -6.0f, 0.0f);    /* D-C */

            // top
            geom.DefineVertex(A);               // A
            geom.DefineColor(terrainColor);
            geom.DefineVertex(B);               // B
            geom.DefineColor(terrainColor);

            // left
            geom.DefineVertex(A);               // A
            geom.DefineColor(terrainColor);
            geom.DefineVertex(D);               // D
            geom.DefineColor(terrainEndColor);

            // bottom
            geom.DefineVertex(C);               // C
            geom.DefineColor(terrainEndColor);
            geom.DefineVertex(D);               // D
            geom.DefineColor(terrainEndColor);

            // right
            geom.DefineVertex(B);               // B
            geom.DefineColor(terrainColor);
            geom.DefineVertex(C);               // C
            geom.DefineColor(terrainEndColor);

            geom.Commit();
        }

        void DrawBackground(int iteration)
        {
            Sprite2D fgSprite;
            Sprite2D bgLayer1Sprite;
            Sprite2D bgLayer2Sprite;
            Sprite2D bgLayer3Sprite;

            // Get random background
            var min = 5;
            var max = 14;
            var backgroundNum = NextRandom(min, max);
            while(_lastBackground.Equals(backgroundNum))
                backgroundNum = NextRandom(min, max);
            _lastBackground = backgroundNum;

            LevelBackgroundResourceData lvlData;
            switch(iteration)
            {
                case 0:
                    lvlData = new LevelBackgroundResourceData(GameInstance, _lvlLandscape, 1);
                    break;
                case 1:
                    lvlData = new LevelBackgroundResourceData(GameInstance, _lvlLandscape, 2);
                    break;
                case 2:
                    lvlData = new LevelBackgroundResourceData(GameInstance, _lvlLandscape, 3);
                    break;
                case 3:
                    lvlData = new LevelBackgroundResourceData(GameInstance, _lvlLandscape, 4);
                    break;
                default:
                    lvlData = new LevelBackgroundResourceData(GameInstance, _lvlLandscape, backgroundNum);
                    break;
            }

            fgSprite = lvlData.ForegroundSprite;
            bgLayer1Sprite = lvlData.Bg1Sprite;
            bgLayer2Sprite = lvlData.Bg2Sprite;
            bgLayer3Sprite = lvlData.Bg3Sprite;

            fgSprite.Rectangle = AssetsCoordinates.Backgrounds.GameplayBackgroundsGeneric.Foreground.ImageRect;
            bgLayer1Sprite.Rectangle = AssetsCoordinates.Backgrounds.GameplayBackgroundsGeneric.Background1.ImageRect;
            bgLayer2Sprite.Rectangle = AssetsCoordinates.Backgrounds.GameplayBackgroundsGeneric.Background2.ImageRect;
            bgLayer3Sprite.Rectangle = AssetsCoordinates.Backgrounds.GameplayBackgroundsGeneric.Background3.ImageRect;

            // Get correct X value
            float scaleRatio = 1.05f;
            float ratioX = (ScreenInfo.DefaultScreenWidth * GameInstance.ScreenInfo.XScreenRatio) / AssetsCoordinates.Backgrounds.GameplayBackgroundsGeneric.Background3.ImageRect.Right * scaleRatio;
            float ratioY = (ScreenInfo.DefaultScreenHeight * GameInstance.ScreenInfo.YScreenRatio) / AssetsCoordinates.Backgrounds.GameplayBackgroundsGeneric.Background3.ImageRect.Bottom * scaleRatio;
            float x = iteration * (Application.PixelSize * bgLayer3Sprite.Rectangle.Right * ratioX) - Application.PixelSize; // FIX FOR pixel offset

            // FOREGROUND
            Node foregroundNode = CreateChild("fg");
            foregroundNode.Position = new Vector3(x, 1.0f * GameInstance.ScreenInfo.YScreenRatio - 3.2f, 0.0f);
            foregroundNode.Scale = new Vector3(ratioX, ratioY, 0.0f);
            StaticSprite2D fgStaticSprite = foregroundNode.CreateComponent<StaticSprite2D>();
            fgStaticSprite.Sprite = fgSprite;

            // BACKGROUND LAYER 1
            Node bg1Node = CreateChild();
            bg1Node.Position = new Vector3(x, 1.0f, 10.0f);
            bg1Node.Scale = new Vector3(ratioX, ratioY, 0.0f);
            StaticSprite2D bgLayer1StaticSprite = bg1Node.CreateComponent<StaticSprite2D>();
            bgLayer1StaticSprite.Sprite = bgLayer1Sprite;
            //Debug.WriteLine("bgLayer1Sprite rectangle: [{0},{1},{2},{3}", bgLayer1Sprite.re, bgLayer1Sprite.Rectangle.Top, bgLayer1Sprite.Rectangle.Right, bgLayer1Sprite.Rectangle.Bottom);

            // BACKGROUND LAYER 2
            Node bg2Node = CreateChild();
            bg2Node.Position = new Vector3(x, 1.0f, 11.0f);
            bg2Node.Scale = new Vector3(ratioX, ratioY, 0.0f);
            StaticSprite2D bgLayer2StaticSprite = bg2Node.CreateComponent<StaticSprite2D>();
            bgLayer2StaticSprite.Sprite = bgLayer2Sprite;

            // BACKGROUND LAYER 3
            Node bg3Node = CreateChild();
            bg3Node.Position = new Vector3(x, _vehicleOffsetY, 12.0f);
            bg3Node.Scale = new Vector3(ratioX, ratioY, 0.0f);
            StaticSprite2D bgLayer3StaticSprite = bg3Node.CreateComponent<StaticSprite2D>();
            bgLayer3StaticSprite.Sprite = bgLayer3Sprite;

            Bg1List.Add(bg1Node);
            Bg2List.Add(bg2Node);
            Bg3List.Add(bg3Node);
            FgList.Add(foregroundNode);
        }

        void GameOver()
        {
            _gameState = GameState.GAME_OVER;
            if(_stopwatch != null)
                _stopwatch.Pause();

            // HIDE HUD
            HideHud(GameInstance.UI.Root.Children);

            var gameOverWindow = new Window();
            GameInstance.UI.Root.AddChild(gameOverWindow);

            // Set Window size and layout settings
            gameOverWindow.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(0));
            gameOverWindow.SetSize(GameInstance.ScreenInfo.SetX(1920), GameInstance.ScreenInfo.SetY(1080));
            gameOverWindow.SetColor(Color.FromHex("#22000000"));
            gameOverWindow.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            gameOverWindow.Name = "GameOverWindow";

            Font font = GameInstance.ResourceCache.GetFont(GameInstance.defaultFont);

            BorderImage levelFailedImg = new BorderImage();
            gameOverWindow.AddChild(levelFailedImg);
            levelFailedImg.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Top);
            levelFailedImg.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(200));
            levelFailedImg.SetSize(GameInstance.ScreenInfo.SetX(729), GameInstance.ScreenInfo.SetY(424));
            levelFailedImg.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Logos.LevelFailed.ResourcePath);
            levelFailedImg.ImageRect = AssetsCoordinates.Logos.LevelFailed.LevelFailedBT;

            // Create the button and center the text onto it
            Button btnRestart = new Button();
            gameOverWindow.AddChild(btnRestart);
            btnRestart.SetStyleAuto(null);
            btnRestart.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            btnRestart.SetPosition(GameInstance.ScreenInfo.SetX(-110), GameInstance.ScreenInfo.SetY(200));
            btnRestart.SetSize(GameInstance.ScreenInfo.SetX(128), GameInstance.ScreenInfo.SetY(128));
            btnRestart.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            btnRestart.ImageRect = AssetsCoordinates.Generic.Icons.Restart;

            var restartText = new Text();
            gameOverWindow.AddChild(restartText);
            restartText.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            restartText.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(200));
            restartText.SetFont(font, 50);
            restartText.Value = "Press        to Restart";

            btnRestart.Pressed += args => {
                CloseGameLevel();
                GameInstance.LaunchScene(GameScenesEnumeration.GAME, _randomLevel);
            };

            Button btnQuit = new Button();
            gameOverWindow.AddChild(btnQuit);
            btnQuit.SetStyleAuto(null);
            btnQuit.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            btnQuit.SetPosition(GameInstance.ScreenInfo.SetX(-30), GameInstance.ScreenInfo.SetY(380));
            btnQuit.SetSize(GameInstance.ScreenInfo.SetX(128), GameInstance.ScreenInfo.SetY(128));
            btnQuit.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            btnQuit.ImageRect = AssetsCoordinates.Generic.Icons.Continue;

            var quitText = new Text();
            gameOverWindow.AddChild(quitText);
            quitText.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            quitText.SetPosition(GameInstance.ScreenInfo.SetX(-10), GameInstance.ScreenInfo.SetY(380));
            quitText.SetFont(font, 50);
            quitText.Value = "Press        to Exit";

            btnQuit.Pressed += args => {
                CloseGameLevel();
                GameInstance.LaunchScene(GameScenesEnumeration.MENU);
            };

            GameInstance.UI.SetFocusElement(gameOverWindow);

            // Update data only if not test level
            if(_randomLevel)
                return;

            // Update failed race status
            var levelData = TrackManager.Instance.SelectedTrackModel;
            levelData.TotalOfFailures += 1;
            levelData.TotalOfPlays += 1;
            TrackManager.Instance.SelectedTrackModel = levelData;

            // Lose experience points
            var lostExp = CharacterLevelData.LostPoints(_distanceData.Distance(_vehicle.MainBody.Node.Position.X));
            var player = CharacterManager.Instance.User;
            player.Experience += lostExp;
            if(player.Experience < 0)
                player.Experience = 0;
            CharacterManager.Instance.User = player;
        }

        void LevelComplete()
        {
            _gameState = GameState.LEVEL_COMPLETE;

            // HIDE HUD
            HideHud(GameInstance.UI.Root.Children);

            var levelCompleteWindow = new Window();
            GameInstance.UI.Root.AddChild(levelCompleteWindow);

            // Set Window size and layout settings
            levelCompleteWindow.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(0));
            levelCompleteWindow.SetSize(GameInstance.ScreenInfo.SetX(1920), GameInstance.ScreenInfo.SetY(1080));
            levelCompleteWindow.SetColor(Color.FromHex("#22000000"));
            levelCompleteWindow.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            levelCompleteWindow.Name = "LevelCompleteWindow";

            BorderImage levelCompleteImg = new BorderImage();
            levelCompleteWindow.AddChild(levelCompleteImg);
            levelCompleteImg.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Top);
            levelCompleteImg.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(200));
            levelCompleteImg.SetSize(GameInstance.ScreenInfo.SetX(1024), GameInstance.ScreenInfo.SetY(424));
            levelCompleteImg.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Logos.LevelComplete.ResourcePath);
            levelCompleteImg.ImageRect = AssetsCoordinates.Logos.LevelComplete.LevelCompleteBT;

            Button btnContinue = new Button();
            levelCompleteWindow.AddChild(btnContinue);
            btnContinue.SetStyleAuto(null);
            btnContinue.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Bottom);
            btnContinue.SetPosition(GameInstance.ScreenInfo.SetX(-100), GameInstance.ScreenInfo.SetY(-100));
            btnContinue.SetSize(GameInstance.ScreenInfo.SetX(160), GameInstance.ScreenInfo.SetY(160));
            btnContinue.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            btnContinue.ImageRect = AssetsCoordinates.Generic.Icons.Continue;

            btnContinue.Pressed += args => {
                // Set post race data
                var postRaceData = new LastPlayedTrack {
                    TrackData = _levelData,
                    Components = _components,
                    Coins = _coins,
                    Time = (int)_stopwatch.GetElapsedTime().TotalMilliseconds,
                    Points = CharacterLevelData.ObtainedPoints((int)_stopwatch.GetElapsedTime().TotalMilliseconds)
                };
                TrackManager.Instance.LastPlayedTrackInfo = postRaceData;
                CloseGameLevel();
                GameInstance.LaunchScene(GameScenesEnumeration.POST_RACE, _randomLevel);
            };

            GameInstance.UI.SetFocusElement(levelCompleteWindow);
        }

        void PauseMenu()
        {
            GameInstance.UI.SetFocusElement(null);

            _gameState = GameState.PAUSE;
            HideHud(GameInstance.UI.Root.Children);
            GameInstance.PauseGame();
            UpdateEnabled = false;

            if(_stopwatch != null)  
                _stopwatch.Pause();

            var pauseWindow = new Window();
            GameInstance.UI.Root.AddChild(pauseWindow);
            GameInstance.UI.SetFocusElement(null);

            // Set Window size and layout settings
            pauseWindow.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(0));
            pauseWindow.SetSize(GameInstance.ScreenInfo.SetX(1920), GameInstance.ScreenInfo.SetY(1080));
            pauseWindow.SetColor(Color.FromHex("#22000000"));
            pauseWindow.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            pauseWindow.Name = "PauseWindow";

            Font font = GameInstance.ResourceCache.GetFont("Fonts/OpenSans-Bold.ttf");

            Window rectangle = new Window();
            pauseWindow.AddChild(rectangle);
            rectangle.Name = "PauseMenu";
            rectangle.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(0));
            rectangle.SetSize(GameInstance.ScreenInfo.SetX(750), GameInstance.ScreenInfo.SetY(550));
            rectangle.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            rectangle.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            rectangle.ImageRect = AssetsCoordinates.Generic.Boxes.PauseMenuBox;

            Text restart = GameText.CreateText(rectangle, GameInstance.ScreenInfo, font, 50, 50, 50, HorizontalAlignment.Left, VerticalAlignment.Top, "Restart");
            restart.SetColor(Color.White);
            Text settings = GameText.CreateText(rectangle, GameInstance.ScreenInfo, font, 50, 50, 0, HorizontalAlignment.Left, VerticalAlignment.Center, "Settings");
            settings.SetColor(Color.White);
            Text quit = GameText.CreateText(rectangle, GameInstance.ScreenInfo, font, 50, 50, -50, HorizontalAlignment.Left, VerticalAlignment.Bottom, "Quit");
            quit.SetColor(Color.White);

            Button btnRestart = GameButton.CreateButton(rectangle, GameInstance.ScreenInfo, -50, 30, 140, 140, HorizontalAlignment.Right, VerticalAlignment.Top);
            btnRestart.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            btnRestart.ImageRect = AssetsCoordinates.Generic.Icons.Restart;

            btnRestart.Pressed += args => {
                CloseGameLevel();
                GameInstance.LaunchScene(GameScenesEnumeration.GAME, _randomLevel);
            };

            Button btnSettings = GameButton.CreateButton(rectangle, GameInstance.ScreenInfo, -50, 0, 140, 140, HorizontalAlignment.Right, VerticalAlignment.Center);
            btnSettings.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            btnSettings.ImageRect = AssetsCoordinates.Generic.Icons.BtnSettings;

            btnSettings.Pressed += args => {
                rectangle.Visible = false;
                SettingsMenu(pauseWindow);
            };

            Button btnQuit = GameButton.CreateButton(rectangle, GameInstance.ScreenInfo, -50, -30, 140, 140, HorizontalAlignment.Right, VerticalAlignment.Bottom);
            btnQuit.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            btnQuit.ImageRect = AssetsCoordinates.Generic.Icons.Continue;

            btnQuit.Pressed += args => {
                pauseWindow.Visible = false;
                pauseWindow.Remove();

                QuitConfirm();
            };

            Button btnContinue = new Button() {
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center,
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(300), GameInstance.ScreenInfo.SetY(-100)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(140), GameInstance.ScreenInfo.SetY(140)),
                Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath),
                ImageRect = AssetsCoordinates.Generic.Icons.BtnPlay
            };
            //btnContinue.SetStyleAuto(null);
            pauseWindow.AddChild(btnContinue);

            btnContinue.Pressed += args => {
                _gameState = GameState.PLAYING;
                pauseWindow.Visible = false;
                pauseWindow.Remove();

                ShowHud(GameInstance.UI.Root.Children);
                GameInstance.UI.SetFocusElement(null);
                GameInstance.ResumeGame();
                UpdateEnabled = true;
                if(_stopwatch != null)
                    _stopwatch.Resume();
            };

            GameInstance.UI.SetFocusElement(pauseWindow);
        }

        void SettingsMenu(Window pauseWindow) {
            Window rectangle = new Window();
            pauseWindow.AddChild(rectangle);
            rectangle.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(0));
            rectangle.SetSize(GameInstance.ScreenInfo.SetX(750), GameInstance.ScreenInfo.SetY(550));
            rectangle.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            rectangle.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            rectangle.ImageRect = AssetsCoordinates.Generic.Boxes.PauseMenuBox;

            Button closeWindow = new Button {
                Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath),
                ImageRect = AssetsCoordinates.Generic.Icons.BntBack,
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(-50), GameInstance.ScreenInfo.SetY(30)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(140), GameInstance.ScreenInfo.SetY(140)),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };
            rectangle.AddChild(closeWindow);

            closeWindow.Pressed += (PressedEventArgs args) => {
                rectangle.Visible = false;
                var window = pauseWindow.GetChild("PauseMenu");
                window.Visible = true;
            };

            // Create sliders for controlling sound and music master volume
            var soundSlider = CreateSlider(40, 190, 400, 100, _soundVolumeSlider, rectangle);
            var audio = GameInstance.Audio;
            soundSlider.Value = audio.GetMasterGain(SoundType.Effect.ToString());
            soundSlider.SliderChanged += (SliderChangedEventArgs args) => {
                audio.SetMasterGain(SoundType.Effect.ToString(), args.Value); 
                SoundManager.Instance.EffectsGain = args.Value;
            };

            var musicSlider = CreateSlider(40, 340, 400, 100, _musicVolumeSlider, rectangle);
            musicSlider.Value = audio.GetMasterGain(SoundType.Music.ToString());
            musicSlider.SliderChanged += (SliderChangedEventArgs args) => {
                audio.SetMasterGain(SoundType.Music.ToString(), args.Value);
                SoundManager.Instance.MusicGain = args.Value;
            };
        }

        void QuitConfirm() 
        {
            var quitWindow = new Window();
            GameInstance.UI.Root.AddChild(quitWindow);
            GameInstance.UI.SetFocusElement(null);

            // Set Window size and layout settings
            quitWindow.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(0));
            quitWindow.SetSize(GameInstance.ScreenInfo.SetX(1920), GameInstance.ScreenInfo.SetY(1080));
            quitWindow.SetColor(Color.FromHex("#22000000"));
            quitWindow.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            quitWindow.Name = "QuitWindow";

            Font font = GameInstance.ResourceCache.GetFont(GameInstance.defaultFont);

            Window rectangle = new Window();
            quitWindow.AddChild(rectangle);
            rectangle.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(0));
            rectangle.SetSize(GameInstance.ScreenInfo.SetX(750), GameInstance.ScreenInfo.SetY(260));
            rectangle.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            rectangle.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            rectangle.ImageRect = AssetsCoordinates.Generic.Boxes.ExitConfirmationBox;

            Text warningText = GameText.CreateText(rectangle, GameInstance.ScreenInfo, font, 35, 250, 0, HorizontalAlignment.Left, VerticalAlignment.Center, "Are you sure? Game progress will be lost.");
            warningText.Wordwrap = true;
            warningText.SetSize(GameInstance.ScreenInfo.SetX(750-270), GameInstance.ScreenInfo.SetY(240));
            warningText.SetColor(Color.White);

            var quitButton = new Button();
            quitButton.SetPosition(GameInstance.ScreenInfo.SetX(-85), GameInstance.ScreenInfo.SetY(200));
            quitButton.SetSize(GameInstance.ScreenInfo.SetX(285), GameInstance.ScreenInfo.SetY(130));
            quitButton.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            quitButton.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            quitButton.ImageRect = AssetsCoordinates.Generic.Boxes.SelectionPositive;
            quitWindow.AddChild(quitButton);

            Text confirmText = GameText.CreateText(quitButton, GameInstance.ScreenInfo, font, 50, 145, -5, HorizontalAlignment.Left, VerticalAlignment.Center, "Yes");
            confirmText.SetColor(Color.White);

            quitButton.Pressed += (PressedEventArgs args) => {
                CloseGameLevel();
                GameInstance.LaunchScene(GameScenesEnumeration.MENU);
            };

            var continueButton = new Button();
            continueButton.SetPosition(GameInstance.ScreenInfo.SetX(235), GameInstance.ScreenInfo.SetY(200));
            continueButton.SetSize(GameInstance.ScreenInfo.SetX(285), GameInstance.ScreenInfo.SetY(130));
            continueButton.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            continueButton.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            continueButton.ImageRect = AssetsCoordinates.Generic.Boxes.SelectionNegative;
            quitWindow.AddChild(continueButton);

            Text cancelText = GameText.CreateText(continueButton, GameInstance.ScreenInfo, font, 50, 145, -5, HorizontalAlignment.Left, VerticalAlignment.Center, "No");
            cancelText.SetColor(Color.White);

            continueButton.Pressed += (PressedEventArgs args) => {
                quitWindow.Visible = false;
                quitWindow.Remove();

                PauseMenu();
            };
        }

        void HideHud(IReadOnlyList<UIElement> children, int level = 0) {
            foreach(var control in children) {
                if(control.TypeName.Equals("Button") || control.TypeName.Equals("BorderImage"))
                    control.Visible = false;

                if (control.Children.Count > 0)
                    HideHud(control.Children, level++);
            }
        }

        void ShowHud(IReadOnlyList<UIElement> children, int level = 0) {
            foreach(var control in children) {
                //Debug.WriteLine("HUD SHOW: level " + level + ": " + control.Name + " - " + control.TypeName);

                if(control.TypeName.Equals("Button") || control.TypeName.Equals("BorderImage"))
                    control.Visible = true;

                if(control.Children.Count > 0)
                    ShowHud(control.Children, level++);
            }
        }

        void CloseGameLevel()
        {
            GameInstance.UI.SetFocusElement(null);
            GameInstance.OSDCommands.Accelerating = false;
            GameInstance.OSDCommands.Braking = false;
            GameInstance.OSDCommands.RotatingLeft = false;
            GameInstance.OSDCommands.RotatingRight = false;
            GameInstance.Engine.PostRenderUpdate -= PostRenderUpdate;
            GameInstance.Engine.PostUpdate -= PostUpdate;
            GameInstance.Bg1Node = null;
            GameInstance.Bg2Node = null;
            GameInstance.Bg3Node = null;
            GameInstance.FgNode = null;
            Bg1List = null;
            Bg2List = null;
            Bg3List = null;
            FgList = null;

            Remove();
            Dispose();
        }

        void InitOSD()
        {
            TouchEnabled = true;

#if DEBUG
            InitShowHidePhysics();
#endif
            InitButtonAccelerate();
            InitButtonBrake();
            InitButtonRotateLeft();
            InitButtonRotateRight();
            InitButtonBoost();
            InitTopHud();
            InitTopHudData();
        }

        void InitShowHidePhysics()
        {
            var btn = GameButton.CreateButton(GameInstance.UI.Root, GameInstance.ScreenInfo, -100, -380, 160, 160, HorizontalAlignment.Right, VerticalAlignment.Bottom);
            btn.Name = "Button999";
            btn.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            btn.ImageRect = AssetsCoordinates.Generic.Icons.ComponentsIcon;
            btn.HoverOffset = new IntVector2(0, 0);
            btn.PressedOffset = new IntVector2(0, 0);

            btn.Pressed += (PressedEventArgs args) => {
                GameInstance.ShowCollisionGeometry = !GameInstance.ShowCollisionGeometry;
            };
        }

        void InitButtonAccelerate()
        {
            if(SettingsManager.Instance.ControllerLayout == ControllerPosition.RIGHT_CONTROLLER)
                btnA = GameButton.CreateButton(GameInstance.UI.Root, GameInstance.ScreenInfo, -100, -190, 160, 160, HorizontalAlignment.Right, VerticalAlignment.Bottom);
            else
                btnA = GameButton.CreateButton(GameInstance.UI.Root, GameInstance.ScreenInfo, 100, -190, 160, 160, HorizontalAlignment.Left, VerticalAlignment.Bottom);

            btnA.Name = "Button0";
            btnA.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            btnA.ImageRect = AssetsCoordinates.Generic.Icons.AButton;
            btnA.HoverOffset = new IntVector2(0, 0);
            btnA.PressedOffset = new IntVector2(0, 0);

            btnA.Pressed += (PressedEventArgs args) => {
                GameInstance.UI.SetFocusElement(null);
                GameInstance.OSDCommands.Accelerating = true;
                Debug.WriteLine("A DOWN");
                if(!_timer) {
                    _stopwatch = new Stopwatch();
                    _timer = true;
                }
            };

            btnA.Released += (ReleasedEventArgs args) => {
                GameInstance.UI.SetFocusElement(null);
                GameInstance.OSDCommands.Accelerating = false;
                Debug.WriteLine("A UP");
            };
        }

        void InitButtonBrake() 
        {
            if(SettingsManager.Instance.ControllerLayout == ControllerPosition.RIGHT_CONTROLLER)
                btnB = GameButton.CreateButton(GameInstance.UI.Root, GameInstance.ScreenInfo, -240, -40, 160, 160, HorizontalAlignment.Right, VerticalAlignment.Bottom);
            else
                btnB = GameButton.CreateButton(GameInstance.UI.Root, GameInstance.ScreenInfo, 240, -40, 160, 160, HorizontalAlignment.Left, VerticalAlignment.Bottom);

            btnB.Name = "Button1";
            btnB.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            btnB.ImageRect = AssetsCoordinates.Generic.Icons.BButton;
            btnB.HoverOffset = new IntVector2(0, 0);
            btnB.PressedOffset = new IntVector2(0, 0);

            btnB.Pressed += (PressedEventArgs args) => {
                GameInstance.UI.SetFocusElement(null);
                GameInstance.OSDCommands.Braking = true;
                Debug.WriteLine("B DOWN");
            };

            btnB.Released += (ReleasedEventArgs args) => {
                GameInstance.UI.SetFocusElement(null);
                GameInstance.OSDCommands.Braking = false;
                Debug.WriteLine("B UP");
            };
        }

        void InitButtonRotateLeft() 
        {
            if(SettingsManager.Instance.ControllerLayout == ControllerPosition.RIGHT_CONTROLLER)
                btnL = GameButton.CreateButton(GameInstance.UI.Root, GameInstance.ScreenInfo, 100, -190, 160, 160, HorizontalAlignment.Left, VerticalAlignment.Bottom);
            else
                btnL = GameButton.CreateButton(GameInstance.UI.Root, GameInstance.ScreenInfo, -100, -190, 160, 160, HorizontalAlignment.Right, VerticalAlignment.Bottom);

            btnL.Name = "Button2";
            btnL.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            btnL.ImageRect = AssetsCoordinates.Generic.Icons.RLeftButton;
            btnL.HoverOffset = new IntVector2(0, 0);
            btnL.PressedOffset = new IntVector2(0, 0);

            btnL.Focused += (FocusedEventArgs obj) => {
                GameInstance.UI.SetFocusElement(null);
                Debug.WriteLine("ROTATE LEFT FOCUSED");
            };

            btnL.Pressed += (PressedEventArgs args) => {
                GameInstance.UI.SetFocusElement(null);
                GameInstance.OSDCommands.RotatingLeft = true;
                Debug.WriteLine("ROTATE LEFT DOWN");
            };

            btnL.Released += (ReleasedEventArgs args) => {
                GameInstance.UI.SetFocusElement(null);
                GameInstance.OSDCommands.RotatingLeft = false;
                Debug.WriteLine("ROTATE LEFT UP");
            };
        }

        void InitButtonRotateRight() 
        {
            if(SettingsManager.Instance.ControllerLayout == ControllerPosition.RIGHT_CONTROLLER)
                btnR = GameButton.CreateButton(GameInstance.UI.Root, GameInstance.ScreenInfo, 240, -40, 160, 160, HorizontalAlignment.Left, VerticalAlignment.Bottom);
            else
                btnR = GameButton.CreateButton(GameInstance.UI.Root, GameInstance.ScreenInfo, -240, -40, 160, 160, HorizontalAlignment.Right, VerticalAlignment.Bottom);

            btnR.Name = "Button3";
            btnR.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            btnR.ImageRect = AssetsCoordinates.Generic.Icons.RRightButton;
            btnR.HoverOffset = new IntVector2(0, 0);
            btnR.PressedOffset = new IntVector2(0, 0);

            btnR.Focused += (FocusedEventArgs obj) => {
                GameInstance.UI.SetFocusElement(null);
                Debug.WriteLine("ROTATE RIGHT FOCUSED");
            };

            btnR.Pressed += (PressedEventArgs args) => {
                GameInstance.UI.SetFocusElement(null);
                GameInstance.OSDCommands.RotatingRight = true;
                Debug.WriteLine("ROTATE RIGHT DOWN");
            };

            btnR.Released += (ReleasedEventArgs args) => {
                GameInstance.UI.SetFocusElement(null);
                GameInstance.OSDCommands.RotatingRight = false;
                Debug.WriteLine("ROTATE RIGHT UP");
            };
        }

        void InitButtonBoost() 
        {
            if(SettingsManager.Instance.ControllerLayout == ControllerPosition.RIGHT_CONTROLLER)
                btnBoost = GameButton.CreateButton(GameInstance.UI.Root, GameInstance.ScreenInfo, 100, 380, 160, 160, HorizontalAlignment.Left, VerticalAlignment.Bottom);
            else
                btnBoost = GameButton.CreateButton(GameInstance.UI.Root, GameInstance.ScreenInfo, -100, 380, 160, 160, HorizontalAlignment.Right, VerticalAlignment.Bottom);

            btnBoost.Name = "Button4";
            btnBoost.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            btnBoost.ImageRect = AssetsCoordinates.Generic.Icons.BoosterIcon;
            btnBoost.HoverOffset = new IntVector2(0, 0);
            btnBoost.PressedOffset = new IntVector2(0, 0);

            btnBoost.Pressed += (PressedEventArgs args) => {
                Debug.WriteLine("BOOST DOWN");
            };

            btnBoost.Released += (ReleasedEventArgs args) => {
                Debug.WriteLine("BOOST UP");
            };
        }

        void InitTopHud() 
        {
            // COIN VALUE CONTAINER
            var coinBox = new BorderImage {
                Name = "coinsBox",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(160), GameInstance.ScreenInfo.SetY(90)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(200), GameInstance.ScreenInfo.SetY(80)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath),
                ImageRect = AssetsCoordinates.Generic.Boxes.CoinsBox,
                HoverOffset = new IntVector2(0, 0),
                Priority = 5
            };
            GameInstance.UI.Root.AddChild(coinBox);

            // COIN ICON
            var coinIcon = new BorderImage {
                Name = "coinsIcon",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(310), GameInstance.ScreenInfo.SetY(80)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(100), GameInstance.ScreenInfo.SetY(100)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath),
                ImageRect = AssetsCoordinates.Generic.Icons.CoinsIcon,
                HoverOffset = new IntVector2(0, 0),
                Priority = 10

            };
            GameInstance.UI.Root.AddChild(coinIcon);
            coinIcon.BringToFront();

            // COMPONENTS VALUE CONTAINER
            var componentsBox = new BorderImage {
                Name = "componentsBox",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(225), GameInstance.ScreenInfo.SetY(200)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(130), GameInstance.ScreenInfo.SetY(80)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath),
                ImageRect = AssetsCoordinates.Generic.Boxes.ComponentsIcon,
                HoverOffset = new IntVector2(0, 0),
                Priority = 1
            };
            GameInstance.UI.Root.AddChild(componentsBox);

            // COMPONENTS ICON
            var componentsIcon = new BorderImage {
                Name = "componentsIcon",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(310), GameInstance.ScreenInfo.SetY(190)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(100), GameInstance.ScreenInfo.SetY(100)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath),
                ImageRect = AssetsCoordinates.Generic.Icons.ComponentsIcon,
                HoverOffset = new IntVector2(0, 0),
                Priority = 10
            };
            GameInstance.UI.Root.AddChild(componentsIcon);
            componentsIcon.BringToFront();

            // MAP CONTAINER
            int mapLengthEnd = 1060;
            int mapLengthStart = 0;
            int mapHeightEnd = 240;

            var mapBox = new BorderImage {
                Name = "mapBox",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(mapLengthStart), GameInstance.ScreenInfo.SetY(70)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(mapLengthEnd), GameInstance.ScreenInfo.SetY(mapHeightEnd)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath),
                ImageRect = AssetsCoordinates.Generic.Boxes.MapBox,
                HoverOffset = new IntVector2(0, 0)
            };
            GameInstance.UI.Root.AddChild(mapBox);

            // Init map position 
            _mapPositionData.Initialize(mapLengthStart, mapLengthEnd, 0, mapHeightEnd, GameInstance.ScreenInfo.XScreenRatio, GameInstance.ScreenInfo.YScreenRatio);

            // MAP INDICATOR BAR
            var mapIndicator = new BorderImage {
                Name = "mapBar",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(2)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(3), GameInstance.ScreenInfo.SetY(236)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath),
                ImageRect = AssetsCoordinates.Generic.Boxes.MapBar,
                HoverOffset = new IntVector2(0, 0)
            };
            mapBox.AddChild(mapIndicator);

            // DISTANCE TRAVELLED CONTAINER
            var distanceBox = new BorderImage {
                Name = "distanceBox",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(320)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(300), GameInstance.ScreenInfo.SetY(80)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath),
                ImageRect = AssetsCoordinates.Generic.Boxes.DistanceBox,
                HoverOffset = new IntVector2(0, 0)
            };
            GameInstance.UI.Root.AddChild(distanceBox);

            // TIME CONTAINER
            var timeBox = new BorderImage {
                Name = "timeBox",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(-130), GameInstance.ScreenInfo.SetY(90)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(260), GameInstance.ScreenInfo.SetY(80)),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath),
                ImageRect = AssetsCoordinates.Generic.Boxes.TimeBestBox,
                HoverOffset = new IntVector2(0, 0),
                Priority = 5
            };
            GameInstance.UI.Root.AddChild(timeBox);
                      
            // TIME ICON
            var timeIcon = new BorderImage {
                Name = "timeIcon",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(-310), GameInstance.ScreenInfo.SetY(80)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(100), GameInstance.ScreenInfo.SetY(100)),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath),
                ImageRect = AssetsCoordinates.Generic.Icons.TimeIcon,
                HoverOffset = new IntVector2(0, 0),
                Priority = 10
            };
            GameInstance.UI.Root.AddChild(timeIcon);
            timeIcon.BringToFront();

            // BEST TIME CONTAINER
            var bestTimeBox = new BorderImage {
                Name = "bestTimeBox",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(-130), GameInstance.ScreenInfo.SetY(200)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(260), GameInstance.ScreenInfo.SetY(80)),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath),
                ImageRect = AssetsCoordinates.Generic.Boxes.TimeBestBox,
                HoverOffset = new IntVector2(0, 0),
                Priority = 5
            };
            GameInstance.UI.Root.AddChild(bestTimeBox);

            // BEST TIME ICON
            var bestTimeIcon = new BorderImage {
                Name = "bestTimeIcon",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(-310), GameInstance.ScreenInfo.SetY(190)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(100), GameInstance.ScreenInfo.SetY(100)),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath),
                ImageRect = AssetsCoordinates.Generic.Icons.BestTimeIcon,
                HoverOffset = new IntVector2(0, 0),
                Priority = 10
            };
            GameInstance.UI.Root.AddChild(bestTimeIcon);
            bestTimeIcon.BringToFront();
        }

        void InitTopHudData()
        {
            // COINS
            var coinsText = new Text {
                Name = "coinsText",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(-50), GameInstance.ScreenInfo.SetY(0)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(180), GameInstance.ScreenInfo.SetY(70)),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Priority = 100,
                Value = "0"
            };
            coinsText.SetFont(GameInstance.defaultFont, 17);
            var coinsBox = GameInstance.UI.Root.GetChild("coinsBox");
            coinsBox.AddChild(coinsText);

            // COMPONENTS
            var componentsText = new Text {
                Name = "componentsText",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(-50), GameInstance.ScreenInfo.SetY(0)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(110), GameInstance.ScreenInfo.SetY(70)),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Priority = 100,
                Value = "00"
            };
            componentsText.SetFont(GameInstance.defaultFont, 17);
            var componentsBox = GameInstance.UI.Root.GetChild("componentsBox");
            componentsBox.AddChild(componentsText);

            // CURRENT TIME
            var currTimeText = new Text {
                Name = "currentTimeText",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(80), GameInstance.ScreenInfo.SetY(0)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(160), GameInstance.ScreenInfo.SetY(70)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Priority = 100,
                Value = TimeSpan.Zero.MillisRepresentation(),
            };
            currTimeText.SetFont(GameInstance.defaultFont, 17);
            var currTimeBox = GameInstance.UI.Root.GetChild("timeBox");
            currTimeBox.AddChild(currTimeText);

            // BEST TIME
            var bestTimeText = new Text {
                Name = "bestTimeText",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(80), GameInstance.ScreenInfo.SetY(0)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(160), GameInstance.ScreenInfo.SetY(70)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Priority = 100,
                Value = TimeSpan.FromMilliseconds(_levelData.BestTime).MillisRepresentation(),
            };
            bestTimeText.SetFont(GameInstance.defaultFont, 17);
            var bestTimeBox = GameInstance.UI.Root.GetChild("bestTimeBox");
            bestTimeBox.AddChild(bestTimeText);

            // DISTANCE TRAVELLED
            var distanceText = new Text {
                Name = "distanceText",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(0)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(280), GameInstance.ScreenInfo.SetY(70)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Priority = 100,
                Value = "0 m",
            };
            distanceText.SetFont(GameInstance.defaultFont, 17);
            var distanceBox = GameInstance.UI.Root.GetChild("distanceBox");
            distanceBox.AddChild(distanceText);

            /*
            // Draw map line
            var mapBox = GameInstance.UI.Root.GetChild("mapBox");
            var mapLineNode = CreateChild("mapLineNode");

            for(var i = 0; i < terrainData.Count - TerrainGenerator.EndOfLevelSurfaceLength; i+=10) {
                Vector2 point1;
                Vector2 point2;
                if(i <= 10) {
                    continue;
                }
                else {
                    var range = terrainData.GetRange(i - 10, 10);
                    point1 = range.First().Vector;
                    point2 = range.Last().Vector;
                    if(point1.X <= 0)
                        continue;
                }
                var map1 = _mapPositionData.TerrainPoint(point1);
                var map2 = _mapPositionData.TerrainPoint(point2);

                var mapLineGeometry = mapLineNode.CreateComponent<CustomGeometry>(CreateMode.Local, (uint)i);
                mapLineGeometry.BeginGeometry(0, PrimitiveType.LineList);
                var material = new Material();
                material.SetTechnique(0, CoreAssets.Techniques.NoTextureUnlitVCol);
                mapLineGeometry.SetMaterial(material);

                var color = new Color(1.0f, 1.0f / 255.0f, 1.0f / 255.0f, 1.0f);
                mapLineGeometry.DefineVertex(new Vector3(map1.X, map1.Y, 0.0f));
                mapLineGeometry.DefineColor(color);
                mapLineGeometry.DefineVertex(new Vector3(map2.X, map2.Y, 0.0f));
                mapLineGeometry.DefineColor(color);

                mapLineGeometry.Commit();
            }
            */
        }

        void PlaceCoin(Vector2 vector) {
            if(_coinPositionedCounter == 0) {
                var compRnd = NextRandom(0, 751);
                if(compRnd == 0 && !_componentsCollected) {
                    _componentsCollected = true;
                    // Place component
                    // Node
                    Node componentNode = CreateChild(_componentCollisionName);
                    componentNode.Position = (new Vector3(vector.X, vector.Y - 1.15f, 0.75f));
                    componentNode.Scale = new Vector3(1.5f * GameInstance.ScreenInfo.XScreenRatio, 1.5f * GameInstance.ScreenInfo.YScreenRatio, 1.0f);
                    StaticSprite2D componentStaticSprite = componentNode.CreateComponent<StaticSprite2D>();
                    componentStaticSprite.Sprite = GameInstance.ResourceCache.GetSprite2D(AssetsCoordinates.Level.Collectible.ResourcePath);
                    componentStaticSprite.Sprite.Rectangle = AssetsCoordinates.Level.Collectible.Wheels;

                    // Collision shape
                    var componentCollision = componentNode.CreateComponent<CollisionBox2D>();
                    componentCollision.Size = new Vector2(0.75f, 0.75f);
                    componentCollision.Friction = 0.25f;
                    componentCollision.Density = 0.001f;
                    componentCollision.Restitution = 0f;

                    // Collision body
                    var componentCenter = componentNode.CreateComponent<RigidBody2D>();
                    componentCenter.BodyType = BodyType2D.Dynamic;
                    componentCenter.Mass = 0.01f;
                    return;
                }

                var rnd = NextRandom(0, 101);
                if(rnd < 80)
                    return;
            }

            if(_coinPositionedCounter == 0)
                _coinPositionedCounter = 5;
            _coinPositionedCounter--;

            // Node
            Node coinNode = CreateChild(_coinCollisionName);
            coinNode.Position = (new Vector3(vector.X, vector.Y - 1.15f, 0.75f));
            coinNode.Scale = new Vector3(1.5f * GameInstance.ScreenInfo.XScreenRatio, 1.5f *  GameInstance.ScreenInfo.YScreenRatio, 1.0f);
            StaticSprite2D coinStaticSprite = coinNode.CreateComponent<StaticSprite2D>();
            coinStaticSprite.Sprite = GameInstance.ResourceCache.GetSprite2D(AssetsCoordinates.Level.Collectible.ResourcePath);
            coinStaticSprite.Sprite.Rectangle = AssetsCoordinates.Level.Collectible.Coin;

            // Collision shape
            var coinCollision = coinNode.CreateComponent<CollisionBox2D>();
            coinCollision.Size = new Vector2(0.75f, 0.75f);// Set radius
            coinCollision.Friction = 0.25f;
            coinCollision.Density = 0.001f;
            coinCollision.Restitution = 0f;

            // Collision body
            var coinCenter = coinNode.CreateComponent<RigidBody2D>();
            coinCenter.BodyType = BodyType2D.Dynamic;
            coinCenter.Mass = 0.01f;

            if(_removedFirstCoin)
                return;

            _removedFirstCoin = true;
            RemoveChild(GetChild(_coinCollisionName));
        }

        Slider CreateSlider(int x, int y, int xSize, int ySize, string text, UIElement container) {
            ResourceCache cache = GameInstance.ResourceCache;
            Font font = cache.GetFont(GameInstance.defaultFont);

            // Create text and slider below it
            var textWidth = 170;
            Sprite SFX = new Sprite {
                Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath),
                ImageRect = AssetsCoordinates.Generic.Boxes.VolumeBarGreen,
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(textWidth), GameInstance.ScreenInfo.SetY(ySize)),
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(x), GameInstance.ScreenInfo.SetY(y)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            Text SFXtext = new Text();
            SFX.AddChild(SFXtext);
            SFXtext.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            SFXtext.SetPosition(GameInstance.ScreenInfo.SetX(-5), 0);
            SFXtext.SetFont(font, GameInstance.ScreenInfo.SetX(30));
            SFXtext.Value = text;

            container.AddChild(SFX);

            Slider slider = new Slider();
            container.AddChild(slider);
            slider.SetStyleAuto(null);
            slider.SetPosition(GameInstance.ScreenInfo.SetX(x + textWidth + 10), GameInstance.ScreenInfo.SetY(y));
            slider.SetSize(GameInstance.ScreenInfo.SetX(xSize), GameInstance.ScreenInfo.SetY(ySize));
            slider.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            slider.ImageRect = AssetsCoordinates.Generic.Boxes.VolumeBarWhite;
            // Use 0-1 range for controlling sound/music master volume
            slider.Range = 1.0f;

            var knob = slider.Knob;
            knob.SetFixedSize(GameInstance.ScreenInfo.SetX(40), GameInstance.ScreenInfo.SetY(ySize));
            knob.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            knob.ImageRect = AssetsCoordinates.Generic.Boxes.VolumeBarKnob;

            return slider;
        }

        public Urho.Color BackgroundFogColor() {
            switch(_lvlLandscape) {
                case 1:
                    return BackgroundColors.BeachColor;
                case 2:
                    return BackgroundColors.MoonColor;
                case 3:
                    return BackgroundColors.SnowyForestColor;
                default:
                    return BackgroundColors.MoonColor;
            }
        }
    }
}
