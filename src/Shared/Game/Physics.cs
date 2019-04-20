using System;
using System.Diagnostics;
using Urho;
using Urho.Gui;
using Urho.Physics;
using Urho.Resources;
using Urho.Urho2D;

namespace SmartRoadSense.Shared
{
    public class Physics : Application {
        Scene scene;
        const uint NumObjects = 1;
        const float CameraDistance = 2.0f;
        const float _speedStep = (float)Math.PI / 2;
        Vehicle vehicle;
        Urho.Resources.ResourceCache cache;
        Graphics graphics;

        protected long start;
        protected long end;
        protected long diff;

        protected Node CameraNode { get; set; }
        protected bool TouchEnabled { get; set; }
        protected const float TouchSensitivity = 2;
        protected float Yaw { get; set; }
        protected float Pitch { get; set; }

        // SCREEN SIZE: 1334x750
        // SCREEN INFO
        ScreenInfoRatio screenInfoRatio;

        static readonly Random random = new Random();
        /// Return a random float between 0.0 (inclusive) and 1.0 (exclusive.)
        public static float NextRandom() { return (float)random.NextDouble(); }
        /// Return a random float between 0.0 and range, inclusive from both ends.
        public static float NextRandom(float range) { return (float)random.NextDouble() * range; }
        /// Return a random float between min and max, inclusive from both ends.
        public static float NextRandom(float min, float max) { return (float)((random.NextDouble() * (max - min)) + min); }
        /// Return a random integer between min and max - 1.
        public static int NextRandom(int min, int max) { return random.Next(min, max); }

        /// <summary>
        /// Joystick XML layout for mobile platforms
        /// </summary>
        protected string JoystickLayoutPatch;// => JoystickLayoutPatches.WithZoomInAndOut;

        Window window;
        UIElement uiRoot;

        bool _showCollisionGeometry;

        [Preserve]
        public Physics(ApplicationOptions appOptions) : base(appOptions) {
        }

        [Preserve]
        public Physics() : base(new ApplicationOptions(assetsFolder: "Data") { Orientation = ApplicationOptions.OrientationType.Landscape }) //iOS only - https://developer.xamarin.com/api/type/Urho.ApplicationOptions/
        {
        }

        static Physics() {
            UnhandledException += Application_UnhandledException1;
        }

        static void Application_UnhandledException1(object sender, Urho.UnhandledExceptionEventArgs e) {
            e.Handled = true;
        }

        #region override methods
        protected override void Start() {
            base.Start();

            InitResourceCache();

            InitTouchInput();

            Input.Enabled = true;
            Input.KeyDown += HandleKeyDown;

            // init cache
            cache = ResourceCache;

            // Init scene
            CreateBackgroundScene();
            CreateForegroundScene();


            uiRoot = UI.Root;
            scene.SetScale(screenInfoRatio.XScreenRatio);

            XmlFile style = cache.GetXmlFile("UI/DefaultStyle.xml");
            uiRoot.SetDefaultStyle(style);

            // Initialize Window
            InitWindow();

            // Init viewport
            SetupViewport();

            SubscribeToEvents();
        }

        void HandleKeyDown(KeyDownEventArgs e) {
            start = DateTime.Now.Ticks;
        }

        void InitResourceCache() {
            ResourceCache.SearchPackagesFirst = false;
            ResourceCache.FileChanged += (FileChangedEventArgs args) => {
                Debug.WriteLine("File Changed: {0}, {1}", args.FileName, args.ResourceName);
            };

            ResourceCache.LoadFailed += (args) => {
                Debug.WriteLine("Load Failed: {0}", args.ResourceName);
            };

            ResourceCache.ResourceNotFound += (args) => {
                Debug.WriteLine("Resource not found: {0}", args.ResourceName);
            };

            Debug.WriteLine("URHO: Initialized Resource Cache");
        }

        void InitTouchInput() {
            graphics = Graphics;
            screenInfoRatio = new ScreenInfoRatio(graphics.Width, graphics.Height);

            TouchEnabled = true;
            var layout = ResourceCache.GetXmlFile("UI/ScreenJoystick_Samples.xml");

            JoystickLayoutPatch = JoystickLayoutPatches.MainGameUI;
            if(!string.IsNullOrEmpty(JoystickLayoutPatch)) {
                XmlFile patchXmlFile = new XmlFile();
                patchXmlFile.FromString(JoystickLayoutPatch);
                layout.Patch(patchXmlFile);
            }
            var screenJoystickIndex = Input.AddScreenJoystick(layout/*, ResourceCache.GetXmlFile("UI/DefaultStyle.xml")*/);
            Input.SetScreenJoystickVisible(screenJoystickIndex, true);
        }

        protected override void OnUpdate(float timeStep) {
            base.OnUpdate(timeStep);

            // Do not move if the UI has a focused element (p.e.: pause menu)
            if(UI != null && UI.FocusElement != null)
                return;

            // Movement speed as world units per second
            const float moveSpeed = 8.0f;
            Camera camera = CameraNode.GetComponent<Camera>();

            if(Input.GetKeyDown(Key.PageUp)) {
                camera.Zoom = camera.Zoom * 1.01f;
            }
            if(Input.GetKeyDown(Key.PageDown)) {
                camera.Zoom = camera.Zoom * 0.99f;
            }
            if(Input.GetKeyDown(Key.W)) {
                //CameraNode.Translate(Vector3.UnitY * moveSpeed * timeStep);
                vehicle.MainBody.ApplyAngularImpulse(-0.5f * moveSpeed * timeStep, true);
                //mainBody.ApplyLinearImpulseToCenter(Vector2.UnitY * moveSpeed * timeStep, true);
                return;
            }
            if(Input.GetKeyDown(Key.S)) {
                vehicle.MainBody.ApplyAngularImpulse(0.5f * moveSpeed * timeStep, true);
                //mainBody.ApplyLinearImpulseToCenter(-Vector2.UnitY * moveSpeed * timeStep, true);
                return;
            }
            if(Input.GetKeyDown(Key.A)) {
                vehicle.MainBody.ApplyLinearImpulseToCenter(-Vector2.UnitX * moveSpeed * timeStep, true);
                //if(vehicle.Wheel1.MotorSpeed < 12 * Math.PI) {
                //    vehicle.Wheel1.MotorSpeed += _speedStep;
                //    vehicle.Wheel2.MotorSpeed += _speedStep;
                //}
                return;
            }
            if(Input.GetKeyDown(Key.D)) {
                vehicle.MainBody.ApplyLinearImpulseToCenter(Vector2.UnitX * moveSpeed * timeStep, true);
                //if(vehicle.Wheel1.MotorSpeed > -12 * Math.PI) {
                //    vehicle.Wheel1.MotorSpeed -= _speedStep;
                //    vehicle.Wheel2.MotorSpeed -= _speedStep;
                //}
                return;
            }
            if(Input.GetKeyPress(Key.G)) {
                _showCollisionGeometry = !_showCollisionGeometry;
                return;
            }
            if(Input.GetKeyPress(Key.X)) {
                Graphics.Close();
                Current.Exit();
            }
        }

        protected override void OnDeleted() {
            base.OnDeleted();
            System.Diagnostics.Debug.WriteLine("OnDeleted");
            Dispose();
        }

        protected override void Stop() {
            base.Stop();
            System.Diagnostics.Debug.WriteLine("Stop");
        }

        #endregion override

        void SetupViewport() {
            var renderer = Renderer;
            renderer.SetViewport(0, new Viewport(Context, scene, CameraNode.GetComponent<Camera>(), null));

            // HD UI Settings
            Options.AdditionalFlags = " -hd";
            Renderer.GetViewport(0).RenderPath.Append(CoreAssets.PostProcess.FXAA2);
        }

        void SubscribeToEvents() {
            Engine.PostUpdate += (args => {
                if(vehicle.MainBody == null)
                    return;

                CameraNode.Position = new Vector3(vehicle.MainBody.Node.Position.X + 1.0f, vehicle.MainBody.Node.Position.Y + 1.0f, vehicle.MainBody.Node.Position.Z);
            });

            Engine.PostRenderUpdate += (args => {
                var debugRendererComp = scene.GetComponent<DebugRenderer>();
                var physicsComponent = scene.GetComponent<PhysicsWorld2D>();
                if(physicsComponent != null && _showCollisionGeometry)
                    physicsComponent.DrawDebugGeometry(debugRendererComp, false);
            });

        }

        void CreateBackgroundScene() {
            scene = new Scene();
            scene.CreateComponent<Octree>();
            scene.CreateComponent<DebugRenderer>();
            scene.CreateComponent<PhysicsWorld2D>();

            // Create camera node
            CameraNode = scene.CreateChild("Camera");
            // Set camera's position
            CameraNode.Position = (new Vector3(0.1f, 0.0f, -2.0f));

            Camera camera = CameraNode.CreateComponent<Camera>();
            camera.Orthographic = true;

            camera.OrthoSize = (float)graphics.Height * PixelSize;
            camera.Zoom = 3.5f * Math.Min(screenInfoRatio.XScreenRatio, screenInfoRatio.YScreenRatio);

            // Create 2D physics world component
            scene.CreateComponent<PhysicsWorld2D>();

            Sprite2D boxSprite = cache.GetSprite2D("Urho2D/Box.png");

            // FOREGROUND
            Sprite2D foregroundSprite = cache.GetSprite2D("Urho2D/foreground.png");
            Node foregroundNode = scene.CreateChild("Foreground");
            foregroundNode.Position = (new Vector3(0.9f, -1.25f, 0.0f));
            foregroundNode.Scale = new Vector3(2.0f, 2.0f, 0.0f);
            StaticSprite2D foregroundStaticSprite = foregroundNode.CreateComponent<StaticSprite2D>();
            foregroundStaticSprite.Sprite = foregroundSprite;

            // BACKGROUND LAYER 1
            Sprite2D bgLayer1Sprite = cache.GetSprite2D("Urho2D/bg_layer1.png");
            Node bgLayer1Node = scene.CreateChild("Background_layer1");
            bgLayer1Node.Position = (new Vector3(0.9f, -1.25f, 0.1f));
            bgLayer1Node.Scale = new Vector3(2.0f, 2.0f, 0.0f);
            StaticSprite2D bgLayer1StaticSprite = bgLayer1Node.CreateComponent<StaticSprite2D>();
            bgLayer1StaticSprite.Sprite = bgLayer1Sprite;

            // BACKGROUND LAYER 2
            Sprite2D backgroundSprite = cache.GetSprite2D("Urho2D/bg_layer2.png");
            Node backgroundNode = scene.CreateChild("Background_layer2");
            backgroundNode.Position = (new Vector3(0.9f, -1.25f, 0.1f));
            backgroundNode.Scale = new Vector3(2.0f, 2.0f, 0.0f);
            StaticSprite2D backgroundStaticSprite = backgroundNode.CreateComponent<StaticSprite2D>();
            backgroundStaticSprite.Sprite = backgroundSprite;

            // Create ground.
            Node groundNode = scene.CreateChild("Ground");
            groundNode.Position = (new Vector3(0.0f, -3.0f, 0.0f));
            //groundNode.Scale = new Vector3(200.0f, 1.0f, 0.0f);

            // Create 2D rigid body for gound
            groundNode.CreateComponent<RigidBody2D>();

            StaticSprite2D groundSprite = groundNode.CreateComponent<StaticSprite2D>();
            groundSprite.Sprite = boxSprite;

            // CHAIN GROUND
            CollisionChain2D collisionChain = groundNode.CreateComponent<CollisionChain2D>();
            collisionChain.VertexCount = 200;
            for(var i = 0; i < 200; i++) {
                Vector2 currVertex;
                if(i > 0) {
                    currVertex = collisionChain.GetVertex((uint)i - 1);
                    collisionChain.SetVertex((uint)i, new Vector2((i / 2.0f) - 5.0f, currVertex.Y + NextRandom(-0.1f, 0.1f)));
                }
                else
                    collisionChain.SetVertex((uint)i, new Vector2((i / 2.0f) - 5.0f, NextRandom(-0.1f, 0.1f)));

            }


            collisionChain.Friction = 0.3f; // Set friction
            collisionChain.Restitution = 0.0f; // Set restitution (no bounce)


            // FLAT BOX GROUND
            // Create box collider for ground
            //CollisionBox2D groundShape = groundNode.CreateComponent<CollisionBox2D>();
            // Set box size
            //groundShape.Size = new Vector2(0.32f, 0.32f);

        }

        void CreateForegroundScene() {
            Sprite2D boxSprite = cache.GetSprite2D("Urho2D/Box.png");
            Sprite2D ballSprite = cache.GetSprite2D("Urho2D/Ball.png");

            for(uint i = 0; i < NumObjects; ++i) {
                Node node = scene.CreateChild("RigidBody");
                //node.Position = (new Vector3(NextRandom(-0.1f, 0.1f), 5.0f + i * 1.0f, 0.0f));
                node.Position = (new Vector3(-1.6f, -2.0f, 0.1f));
                node.Scale = new Vector3(1.0f, 1.0f, 0.0f);

                // Create rigid body
                RigidBody2D body = node.CreateComponent<RigidBody2D>();
                body.BodyType = BodyType2D.Dynamic;

                StaticSprite2D staticSprite = node.CreateComponent<StaticSprite2D>();
                staticSprite.Sprite = boxSprite;

                // Create box
                CollisionBox2D box = node.CreateComponent<CollisionBox2D>();
                // Set size
                box.Size = new Vector2(0.32f, 0.32f);
                // Set density
                box.Density = 1.0f;
                // Set friction
                box.Friction = 0.5f;
                // Set restitution
                box.Restitution = 0.3f;
            }

            // ADD VEHICLE
            var vehicleMain = new VehicleCreator(scene, cache, screenInfoRatio);
            vehicle = vehicleMain.InitCarInScene(VehicleLoad.BOX);
        }

        void InitWindow() {
            // Create the Window and add it to the UI's root node
            window = new Window();
            uiRoot.AddChild(window);
            // Set Window size and layout settings
            window.SetPosition(184, 68);
            window.SetSize(78, 40);
            window.SetColor(Color.Transparent);
            //window.SetLayout(LayoutMode.Horizontal, 0, new IntRect(3, 3, 3, 3));
            window.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            window.Name = "Window";

            // Create Window 'titlebar' container
            UIElement titleBar = new UIElement();
            titleBar.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Center);
            //titleBar.SetLayout(LayoutMode.Horizontal, 0, new IntRect(0, 0, 0, 0));

            // Create the Window title Text
            var windowTitle = new Text {
                Name = "WindowCoins",
                Value = "9999",
                TextAlignment = HorizontalAlignment.Right
            };

            windowTitle.SetFont("Fonts/Anonymous Pro.ttf", 20);
            windowTitle.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Center);

            // Add the controls to the title bar
            titleBar.AddChild(windowTitle);

            // Add the title bar to the Window
            window.AddChild(titleBar);

            // Apply styles
            window.SetStyleAuto(null);
            windowTitle.SetStyleAuto(null);

            /////////
            CreateUserControls();
        }

        #region GUI 
        // USER CONTROLS
        void CreateUserControls() {
            var accelerate = new Button {
                Name = "Button_Accelerate",
                Position = new IntVector2((int)(screenInfoRatio.XScreenRatio * -96), (int)(screenInfoRatio.XScreenRatio * -128))
            };
            accelerate.SetStyle("Button0");
            accelerate.Pressed += args => 
            {
                Debug.WriteLine("Pressed");
            };

        }
        #endregion
    }
}
