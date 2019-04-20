using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Urho;
using Urho.Gui;
using Urho.Resources;
using Urho.Urho2D;

namespace SmartRoadSense.Shared
{
    public class Game : Application {
        // Game Global variables
        public ScreenInfoRatio ScreenInfo { get; set; }
        public bool ShowCollisionGeometry;
        public bool IsRunningGame;
        public OSDCommands OSDCommands = new OSDCommands();

        public string defaultFont = "Fonts/OpenSans-Bold.ttf";

        [Preserve]
        public Game(ApplicationOptions appOptions) : base(appOptions) {
        }

        [Preserve]
        public Game() : base(new ApplicationOptions(assetsFolder: "Data") { Orientation = ApplicationOptions.OrientationType.Landscape }) //iOS only - https://developer.xamarin.com/api/type/Urho.ApplicationOptions/
        {
        }

        static Game() {
            UnhandledException += Application_UnhandledException;
        }

        static void Application_UnhandledException(object sender, Urho.UnhandledExceptionEventArgs e) {
            e.Handled = true;
        }

        /// <summary>
        /// Joystick XML layout for mobile platforms
        /// </summary>
        public string JoystickLayoutPatch;// => JoystickLayoutPatches.WithZoomInAndOut;

        //DebugHud _debug;
        protected override void Start() {

            InitResourceCache();

            InitUiInfo();

            InitLoadingScreen();

            Task.Run(async () => { await InitTracks(); }).Wait();

            LaunchScene(GameScenesEnumeration.MENU);

            JsonReaderVehicles.GetVehicleConfig();

            CharacterLevelData.PointsToNextLevel();

            VehicleManager.Instance.Init();

#if DEBUG
            // TEST DATA FOR BUYING VEHICLES

            CharacterManager.Instance.Wallet = 100000;

            var newComponentsContainer = new CollectedComponentsContainer {
                CollectedComponentsList = new List<CollectedComponents>()
            };
            var components = VehicleManager.Instance.CollectedComponents;
            foreach(var c in components.CollectedComponentsList) {
                var newComponents = new CollectedComponents {
                    VehicleId = c.VehicleId,
                    VehicleComponents = new Components()
                };
                newComponents.VehicleComponents.Brakes = true;
                newComponents.VehicleComponents.Performance = true;
                newComponents.VehicleComponents.Suspensions = true;
                newComponents.VehicleComponents.Wheels = true;
                newComponentsContainer.CollectedComponentsList.Add(newComponents);
            }
            VehicleManager.Instance.CollectedComponents = newComponentsContainer;

            //_debug = Engine.CreateDebugHud();
            //_debug.ToggleAll();
#endif
        }

        void InitResourceCache() {
            ResourceCache.ReleaseAllResources();
            ResourceCache.AutoReloadResources = true;
            ResourceCache.SearchPackagesFirst = false;

            ResourceCache.FileChanged += (FileChangedEventArgs args) => {
                Debug.WriteLine("RESOURCE CACHE - File Changed: " + args.FileName + ", " + args.ResourceName);
            };

            ResourceCache.LoadFailed += (args) => {
                Debug.WriteLine("RESOURCE CACHE - Load Failed: " + args.ResourceName);
            };

            ResourceCache.ResourceNotFound += (args) => {
                Debug.WriteLine("RESOURCE CACHE - Resource not found: " + args.ResourceName);
            };

            Debug.WriteLine("RESOURCE CACHE - Initialized Resource Cache");
        }

        public void InitUiInfo() {
            Options.AdditionalFlags = " -hd";

            Input.Enabled = true;
            Input.KeyDown += HandleKeyDown;

            ScreenInfo = new ScreenInfoRatio(Graphics.Width, Graphics.Height);

            XmlFile uiStyle = ResourceCache.GetXmlFile("UI/DefaultStyle.xml");
            UI.Root.SetDefaultStyle(uiStyle);

            Engine.MaxFps = 30;
        }

        void InitLoadingScreen() {
            var splashScreen = new BorderImage {
                Texture = ResourceCache.GetTexture2D(AssetsCoordinates.Backgrounds.LoadingGameScreen.ResourcePath),
                ImageRect = AssetsCoordinates.Backgrounds.LoadingGameScreen.ImageRect,
                Size = new IntVector2(ScreenInfo.SetX(1920), ScreenInfo.SetY(1080)),
                Position = new IntVector2(ScreenInfo.SetX(0), ScreenInfo.SetY(0))
            };
            UI.Root.AddChild(splashScreen);
            Engine.RunFrame();
        }

        async Task<bool> InitTracks() {
#if DEBUG
            //TrackManager.Instance.Tracks = null;
#endif
            return await TrackManager.Instance.Init();
        }

        void HandleKeyDown(KeyDownEventArgs e) {
            // Handle Key Down Events
        }

        protected override void OnUpdate(float timeStep) {
            base.OnUpdate(timeStep);

            if(IsRunningGame) {
                GameUpdate(timeStep);
            }
        }

        public void LaunchScene(GameScenesEnumeration sceneNum, bool modifier = false) {
            switch(sceneNum) {
                case GameScenesEnumeration.GAME:
                    SceneManager.Instance.SetScene(new SceneGame(this, modifier));
                    break;
                case GameScenesEnumeration.MENU:
                    SceneManager.Instance.SetScene(new SceneMenu(this));
                    break;
                case GameScenesEnumeration.STORE:
                    break;
                case GameScenesEnumeration.GARAGE:
                    SceneManager.Instance.SetScene(new SceneGarage(this));
                    break;
                case GameScenesEnumeration.PROFILE:
                    SceneManager.Instance.SetScene(new SceneProfileImage(this, modifier));
                    break;
                case GameScenesEnumeration.SETTINGS:
                    SceneManager.Instance.SetScene(new SceneSettings(this));
                    break;
                case GameScenesEnumeration.MULTIPLAYER:
                    break;
                case GameScenesEnumeration.LOADING_SCREEN:
                    SceneManager.Instance.SetScene(new SceneLoadingScreen(this));
                    break;
                case GameScenesEnumeration.LEVEL_SELECT:
                    SceneManager.Instance.SetScene(new SceneLevelSelect(this));
                    break;
                case GameScenesEnumeration.POST_RACE:
                    SceneManager.Instance.SetScene(new ScenePostRace(this, modifier));
                    break;
                case GameScenesEnumeration.USER_PROFILE:
                    SceneManager.Instance.SetScene(new SceneUserProfile(this));
                    break;
                case GameScenesEnumeration.CREDITS:
                    SceneManager.Instance.SetScene(new SceneCredits(this));
                    break;
            }
        }

        public Node CameraNode { get; set; }
        public Vehicle GameVehicle { get; set; }
        public Vehicle GameVehiclePause { get; set; }

        public List<Node> FgNode { get; set; }
        public List<Node> Bg1Node { get; set; }
        public List<Node> Bg2Node { get; set; }
        public List<Node> Bg3Node { get; set; }

        public Action<PostUpdateEventArgs> PostUpdate { get; set; }
        public Action<PostRenderUpdateEventArgs> PostRenderUpdate { get; set; }

        public bool GamePaused;

        /// <summary>
        /// Game main cycle - used mainly for actual game and not for any menu
        /// </summary>
        /// <param name="timeStep">Time step.</param>
        void GameUpdate(float timeStep) {
            if(Bg3Node == null)
                return;

            if(GamePaused)
                return;

            //Debug.WriteLine(_debug.ModeText.Value);
            //Debug.WriteLine(_debug.MemoryText.Value);
            //Debug.WriteLine(_debug.ProfilerText.Value);
            //Debug.WriteLine("\n");

            var staticX = GameVehicle.MainBody.LinearVelocity.X * timeStep;

            if(CameraNode.Position.X >= 0.0f && GameVehicle.MainBody.LinearVelocity.X > 0.01) {
                for(var i = 0; i < Bg3Node.Count; i++) {
                    Bg3Node[i].Position = new Vector3(Bg3Node[i].Position.X + staticX * 0.8f, CameraNode.Position.Y, Bg3Node[i].Position.Z);
                    Bg2Node[i].Position = new Vector3(Bg2Node[i].Position.X + staticX * 0.55f, CameraNode.Position.Y, Bg2Node[i].Position.Z);
                    Bg1Node[i].Position = new Vector3(Bg1Node[i].Position.X + staticX * 0.3f, CameraNode.Position.Y, Bg1Node[i].Position.Z);
                }
            }

            // Do not move if the UI has a focused element (p.e.: pause menu)
            if(UI != null && UI.FocusElement != null)
                return;

            // Movement speed as world units per second
            float moveSpeed = 1.5f * timeStep;
            float brakeSpeed = 1.5f * timeStep;
            moveSpeed *= VehicleManager.Instance.SelectedVehicleModel.Performance;
            brakeSpeed *= VehicleManager.Instance.SelectedVehicleModel.Brake;

            Camera camera = CameraNode.GetComponent<Camera>();
            //camera.Zoom = camera.Zoom * 0.999f;
            //if (Input.GetKeyDown(Key.W))
            //{
            //    camera.Zoom = camera.Zoom * 1.01f;
            //}
            //if (Input.GetKeyDown(Key.S))
            //{
            //    camera.Zoom = camera.Zoom * 0.99f;
            //}
            if(OSDCommands.RotatingLeft) {
                GameVehicle.MainBody.ApplyAngularImpulse(0.5f * moveSpeed, true);
            }
            if(OSDCommands.RotatingRight) {
                GameVehicle.MainBody.ApplyAngularImpulse(-0.5f * moveSpeed, true);
            }
            if(OSDCommands.Braking) {
                if(GameVehicle.MainBody.LinearVelocity.X > 0.0f)
                    GameVehicle.MainBody.ApplyLinearImpulseToCenter(-Vector2.UnitX * brakeSpeed, true);
                else if(GameVehicle.MainBody.LinearVelocity.X < 0.0f)
                    GameVehicle.MainBody.ApplyLinearImpulseToCenter(Vector2.UnitX * brakeSpeed, true);
                else
                    GameVehicle.MainBody.SetLinearVelocity(new Vector2(0, 0));
                return;
            }
            if(OSDCommands.Accelerating) {
                GameVehicle.MainBody.ApplyLinearImpulseToCenter(Vector2.UnitX * moveSpeed, true);
                return;
            }
            if(Input.GetKeyPress(Key.G)) {
                ShowCollisionGeometry = !ShowCollisionGeometry;
                return;
            }

            if(Input.GetKeyPress(Key.X)) {
                // Destroy everything game related
                Engine.PostRenderUpdate -= PostRenderUpdate;
                Engine.PostUpdate -= PostUpdate;
                FgNode = null;
                Bg1Node = null;
                Bg2Node = null;
                Bg3Node = null;

                LaunchScene(GameScenesEnumeration.MENU);
            }
        }

        // Vars for debug window
        Text backWheelSpeedText;

        #region development data
        /// <summary>
        /// Inits the game debug window.
        /// </summary>
        public void InitGameDebugWindow() {

            // Remove Focus or game will think pause menu is open
            UI.SetFocusElement(null);

            // Create the Window and add it to the UI's root node
            var window = new Window();
            UI.Root.AddChild(window);

            // Set Window size and layout settings
            window.SetPosition(-150, -50);
            window.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Center);
            window.SetSize(550, 150);
            window.SetColor(new Color(0.2f, 0.2f, 0.2f, 0.5f));
            window.Name = "WindowDebug";

            // Create the Window title Text
            backWheelSpeedText = new Text {
                Name = "back_wheel_speed",
                Value = "0.0",
                TextAlignment = HorizontalAlignment.Right
            };

            backWheelSpeedText.SetFont("Fonts/Anonymous Pro.ttf", 20);
            backWheelSpeedText.SetColor(Color.Green);
            backWheelSpeedText.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Center);

            // Add the title bar to the Window
            window.AddChild(backWheelSpeedText);

            // Apply styles
            window.SetStyleAuto(null);
            backWheelSpeedText.SetStyleAuto(null);
        }

        /// <summary>
        /// Draws controls for testing vehicle response.
        /// </summary>
        public void DrawDebugControls() {

            var debugInfo = new Window();
            UI.Root.AddChild(debugInfo);

            // Remove Focus or game will think pause menu is open
            UI.SetFocusElement(null);
            Font font = ResourceCache.GetFont("Fonts/OpenSans-Bold.ttf");

            // Set Window size and layout settings
            debugInfo.SetPosition(ScreenInfo.SetX(0), ScreenInfo.SetY(0));
            debugInfo.SetSize(ScreenInfo.SetX(1920), ScreenInfo.SetY(540));
            debugInfo.SetColor(Color.Transparent);
            debugInfo.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            debugInfo.Name = "GameDebugWindow";

            Text suspension = GameText.CreateText(debugInfo, ScreenInfo, font, 20, 210, 50, HorizontalAlignment.Left, VerticalAlignment.Top);
            //var comp = GameVehicle.Wheel1.OtherBody.GetComponent<CollisionCircle2D>();
            //var comp2 = GameVehicle.Wheel2.OtherBody.GetComponent<CollisionCircle2D>();

            //suspension.Value = string.Format("suspensions: {0}, {1}", comp.Restitution, comp2.Restitution);

            // SUSPENTIONS - plus button
            var susButtonPlus = GameButton.CreateButton(debugInfo, ScreenInfo, 100, 0, 100, 100, HorizontalAlignment.Left, VerticalAlignment.Top);

            Text buttonText = GameText.CreateText(susButtonPlus, ScreenInfo, font, 40, 0, 0, HorizontalAlignment.Center, VerticalAlignment.Center);
            buttonText.Value = "+";

            susButtonPlus.Pressed += args => {
                Debug.WriteLine("pressed suspension +");
            };

            // SUSPENTIONS - minus button
            var susButtonMinus = GameButton.CreateButton(debugInfo, ScreenInfo, 0, 0, 100, 100, HorizontalAlignment.Left, VerticalAlignment.Top);

            Text susTextMinus = GameText.CreateText(susButtonMinus, ScreenInfo, font, 40, 0, 0, HorizontalAlignment.Center, VerticalAlignment.Center);
            susTextMinus.Value = "-";

            susButtonMinus.Pressed += args => {
                Debug.WriteLine("pressed suspension -");
            };
        }

        #endregion

        public void PauseGame()
        {
            GamePaused = true;
            // STOP BG
        }

        public void ResumeGame()
        {
            // RESUME BG
            GamePaused = false;
        }

    }
}
