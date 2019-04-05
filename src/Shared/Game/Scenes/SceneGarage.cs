using System;
using Urho;
using Urho.Gui;
using Urho.Resources;

namespace SmartRoadSense.Shared {
    public class SceneGarage : BaseScene {
        ScreenInfoRatio dim; //variabile rapporto dimensioni schermo
        ResourceCache cache;
        Sprite img_vehicle;
        Sprite black_bar;
        Sprite cont_upgrade;
        Sprite upgrademenu;
        BorderImage performance_bar;
        Sprite performance_bar_a;
        Sprite selectable;
        BorderImage suspensions_bar;
        Sprite suspensions_bar_a;
        Sprite cont_downBar;
        BorderImage brake_bar;
        Sprite brake_bar_a;
        Text screen_info;
        BorderImage wheel_bar;
        Sprite wheel_bar_a;
        Text CarName;
        Sprite backgroundSprite;
        UI ui;
        Text buttonText;
        Font font;
        UIElement root;
        int vehicle_selected;
        int vehicle_id;
        Button bt_confirm;
        int vehicles_count;
        Window window;
        Button SelectedVehicle;
        Button NextVehicle;
        Button PrevVehicle;
        Sprite LockedVehicle;
        Sprite cont_components;



        public SceneGarage(Game game) : base(game) {
            vehicle_id = VehicleManager.Instance.SelectedVehicleId;
            if(vehicle_id < 0)
                vehicle_id = 0;

            dim = GameInstance.ScreenInfo;
            root = GameInstance.UI.Root;
            cache = GameInstance.ResourceCache;
            font = cache.GetFont("Fonts/OpenSans-Bold.ttf");
            ui = GameInstance.UI;
            JsonReaderVehicles.GetVehicleConfig();
            var garage_bts = cache.GetTexture2D("Textures/Garage/garage_bts.png");
            vehicles_count = VehicleManager.Instance.VehicleCount;
            vehicle_selected = VehicleManager.Instance.CurrentGarageVehicleId;
            CreateUI();
        }

        

        private void CreateUI() {
            CreateBackground();
            CreateTopBar();

            //CreateVehicleBar();
            CreateNewVehicleBar();
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

            black_bar = root.CreateSprite();
            root.AddChild(black_bar);
            black_bar.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.TopBar.ResourcePath);
            black_bar.Opacity = 0.5f;
            black_bar.SetPosition(0, (int)(dim.YScreenRatio * 30));
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
            
            wallet.Value = ""+ wallet_tot;
            

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
            buttonTitleText.SetPosition(0, 0);
            buttonTitleText.SetFont(font, dim.XScreenRatio * 30);
            buttonTitleText.Value = "VEHICLE SELECT";

            screen_info = new Text();
            screen_title.AddChild(screen_info);
            screen_info.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Center);
            screen_info.SetPosition((int)(dim.XScreenRatio * 0), (int)(dim.YScreenRatio * 90));
            screen_info.SetFont(font, dim.XScreenRatio * 20);
            screen_info.SetColor(Color.White);
            if(VehicleManager.Instance.SelectedVehicleId == -1) {
                screen_info.Value = "Please select a free starting vehicle.";
            }
            else if(VehicleManager.Instance.SelectedVehicleId == vehicle_id) {
                screen_info.Value = "";
                //screen_info.Value = "Selected vehicle.";
            }
            else {
                screen_info.Value = "";
                //screen_info.Value = "Tap to unlock this vehicle";
            }
        }

        void CreateNewVehicleBar() {

            var vehicleBackgroung = cache.GetTexture2D(AssetsCoordinates.Generic.Garage.VehicleBackgroundBar.Path);
            var vehicle = cache.GetTexture2D("Textures/Garage/vehicles2.png");

            Sprite VehicleBar = root.CreateSprite();
            VehicleBar.Texture = vehicleBackgroung;
            VehicleBar.SetSize((int)(dim.XScreenRatio * 1920), (int)(dim.YScreenRatio * 480));
            VehicleBar.SetPosition((int)(dim.XScreenRatio * 0), (int)(dim.YScreenRatio * 300));
            VehicleBar.ImageRect = AssetsCoordinates.Generic.Garage.VehicleBackgroundBar.Rectangle;
            VehicleBar.UseDerivedOpacity = false;
            VehicleBar.Opacity = 0.75f;

            PrevVehicle = new Button();
            VehicleBar.AddChild(PrevVehicle);
            PrevVehicle.Texture = vehicle;
            PrevVehicle.Opacity = 0.7f;
            PrevVehicle.UseDerivedOpacity = false;
            PrevVehicle.SetSize((int)(dim.XScreenRatio * 450), (int)(dim.YScreenRatio * 450));
            PrevVehicle.SetPosition((int)(dim.XScreenRatio * 0), (int)(dim.YScreenRatio * 0));
            PrevVehicle.Pressed += args => {
                Next_vehicle(0);
            };

            // Selected Vehicle image - root element
            SelectedVehicle = new Button();
            VehicleBar.AddChild(SelectedVehicle);
            SelectedVehicle.UseDerivedOpacity = false;
            SelectedVehicle.Texture = vehicle;
            JsonReaderVehicles.SelectSingleVehicle(vehicle_id);
            SelectedVehicle.SetSize((int)(dim.XScreenRatio * 600), (int)(dim.YScreenRatio * 600));
            SelectedVehicle.SetPosition((int)(dim.XScreenRatio * 650), (int)(dim.YScreenRatio * -100));
            SelectedVehicle.Pressed += args => {
                VehicleManager.Instance.CurrentGarageVehicleId = vehicle_id;
                VehicleManager.Instance.SelectedVehicleId = vehicle_id;
                JsonReaderVehicles.SelectSingleVehicle(vehicle_id); // Updates selected vehicle model
                System.Diagnostics.Debug.WriteLine("SAVED ID = " + vehicle_id);
                GameInstance.LaunchScene(GameScenesEnumeration.MENU);
                
            };

            CarName = new Text();
            SelectedVehicle.AddChild(CarName);
            CarName.SetPosition((int)(dim.XScreenRatio * 0), (int)(dim.YScreenRatio * -100));
            CarName.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Bottom);
            CarName.SetFont(font, dim.XScreenRatio * 40);

            LockedVehicle = new Sprite();
            SelectedVehicle.AddChild(LockedVehicle);
            LockedVehicle.SetPosition((int)(dim.XScreenRatio * 650), (int)(dim.YScreenRatio * 200));
            LockedVehicle.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            LockedVehicle.ImageRect = AssetsCoordinates.Generic.Icons.IconLocked;
            LockedVehicle.SetSize((int)(dim.XScreenRatio * 80), (int)(dim.YScreenRatio * 80));

            NextVehicle = new Button();
            NextVehicle.UseDerivedOpacity = false;
            VehicleBar.AddChild(NextVehicle);
            NextVehicle.Texture = vehicle;
            NextVehicle.Opacity = 0.7f;
            JsonReaderVehicles.SelectSingleVehicle(vehicle_id + 1);
            NextVehicle.SetSize((int)(dim.XScreenRatio * 450), (int)(dim.YScreenRatio * 450));
            NextVehicle.SetPosition((int)(dim.XScreenRatio * 1400), (int)(dim.YScreenRatio * 0));
            NextVehicle.Pressed += args => {
                Next_vehicle(1);
            };

            CreateUpgradeBars();

        }

        void CreateUpgradeBars() {
            var garage_bts = cache.GetTexture2D("Textures/Garage/garage_bts.png");
            var green_bars = cache.GetTexture2D("Textures/Garage/green_bars.png");
            var cont_base = cache.GetTexture2D("Textures/Garage/cont_base.png");
            var vehicle = cache.GetTexture2D("Textures/Garage/vehicles2.png");
            cont_upgrade = root.CreateSprite();
            cont_upgrade.Texture = cont_base;
            cont_upgrade.SetSize((int)(dim.XScreenRatio * 1200), (int)(dim.YScreenRatio * 300));
            cont_upgrade.SetPosition((int)(dim.XScreenRatio * 0), (int)(dim.YScreenRatio * 700));
            cont_upgrade.ImageRect = new IntRect(0, 0, 56, 56);

            performance_bar = new BorderImage();
            cont_upgrade.AddChild(performance_bar);
            performance_bar.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            performance_bar.ImageRect = AssetsCoordinates.Generic.Boxes.BoxPerformanceUpgrade;
            performance_bar.SetSize((int)(dim.XScreenRatio * 550), (int)(dim.YScreenRatio * 95));
            performance_bar.SetPosition((int)(dim.XScreenRatio * 1000), (int)(dim.YScreenRatio * 100));

            suspensions_bar = new BorderImage();
            cont_upgrade.AddChild(suspensions_bar);
            suspensions_bar.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            suspensions_bar.ImageRect = AssetsCoordinates.Generic.Boxes.BoxSuspensionUpgrade;
            suspensions_bar.SetSize((int)(dim.XScreenRatio * 550), (int)(dim.YScreenRatio * 95));
            suspensions_bar.SetPosition((int)(dim.XScreenRatio * 1000), (int)(dim.YScreenRatio * 200));

            wheel_bar = new BorderImage();
            cont_upgrade.AddChild(wheel_bar);
            wheel_bar.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            wheel_bar.ImageRect = AssetsCoordinates.Generic.Boxes.BoxWheelUpgrade;
            wheel_bar.SetSize((int)(dim.XScreenRatio * 550), (int)(dim.YScreenRatio * 95));
            wheel_bar.SetPosition((int)(dim.XScreenRatio * 440), (int)(dim.YScreenRatio * 100));
            

            brake_bar = new BorderImage();
            cont_upgrade.AddChild(brake_bar);
            brake_bar.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            brake_bar.ImageRect = AssetsCoordinates.Generic.Boxes.BoxBrakeUpgrade;
            brake_bar.SetSize((int)(dim.XScreenRatio * 550), (int)(dim.YScreenRatio * 95));
            brake_bar.SetPosition((int)(dim.XScreenRatio * 440), (int)(dim.YScreenRatio * 200));


            cont_components = root.CreateSprite();
            cont_components.Texture = cont_base;
            cont_components.SetSize((int)(dim.XScreenRatio * 1200), (int)(dim.YScreenRatio * 300));
            cont_components.SetPosition((int)(dim.XScreenRatio * 450), (int)(dim.YScreenRatio * 700));
            cont_components.ImageRect = new IntRect(0, 0, 56, 56);


            BorderImage Body = new BorderImage();
            cont_components.AddChild(Body);
            Body.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            Body.ImageRect = AssetsCoordinates.Generic.Boxes.ComponentBodyRed;
            Body.SetSize((int)(dim.XScreenRatio * 150), (int)(dim.YScreenRatio * 150));
            Body.SetPosition((int)(dim.XScreenRatio * 100), (int)(dim.YScreenRatio * 150));

            BorderImage Drivetrain = new BorderImage();
            cont_components.AddChild(Drivetrain);
            Drivetrain.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            Drivetrain.ImageRect = AssetsCoordinates.Generic.Boxes.ComponentDrivetrainRed;
            Drivetrain.SetSize((int)(dim.XScreenRatio * 150), (int)(dim.YScreenRatio * 150));
            Drivetrain.SetPosition((int)(dim.XScreenRatio * 300), (int)(dim.YScreenRatio * 150));


            BorderImage Engine = new BorderImage();
            cont_components.AddChild(Engine);
            Engine.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            Engine.ImageRect = AssetsCoordinates.Generic.Boxes.ComponentEngineRed;
            Engine.SetSize((int)(dim.XScreenRatio * 150), (int)(dim.YScreenRatio * 150));
            Engine.SetPosition((int)(dim.XScreenRatio * 500), (int)(dim.YScreenRatio * 150));

            BorderImage Suspensions = new BorderImage();
            cont_components.AddChild(Suspensions);
            Suspensions.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            Suspensions.ImageRect = AssetsCoordinates.Generic.Boxes.ComponentSuspensionRed;
            Suspensions.SetSize((int)(dim.XScreenRatio * 150), (int)(dim.YScreenRatio * 150));
            Suspensions.SetPosition((int)(dim.XScreenRatio * 700), (int)(dim.YScreenRatio * 150));

            BorderImage Whell = new BorderImage();
            cont_components.AddChild(Whell);
            Whell.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            Whell.ImageRect = AssetsCoordinates.Generic.Boxes.ComponentWhellRed;
            Whell.SetSize((int)(dim.XScreenRatio * 150), (int)(dim.YScreenRatio * 150));
            Whell.SetPosition((int)(dim.XScreenRatio * 900), (int)(dim.YScreenRatio * 150));

            SetUpgrade();
            GetCarImg();
        }

        void SetUpgrade() {
            var green_bars = cache.GetTexture2D("Textures/Garage/green_bars.png");
            int perf = VehicleManager.Instance.SelectedVehicleModel.Performance;
            int whe = VehicleManager.Instance.SelectedVehicleModel.Wheel;
            int susp = VehicleManager.Instance.SelectedVehicleModel.Suspensions;
            int brk = VehicleManager.Instance.SelectedVehicleModel.Brake;
            // pref, whe, sup or brk *20 equals the X Size of the performance bar.
            int performance = perf * 20;
            int wheels = whe * 20;
            int suspensions = susp * 20;
            int brake = brk * 20;

            performance_bar_a = new Sprite(); 
            performance_bar.AddChild(performance_bar_a);
            performance_bar_a.Texture = green_bars;
            performance_bar_a.SetPosition((int)(dim.XScreenRatio * 90), (int)(dim.YScreenRatio * 16));
            performance_bar_a.SetSize((int)(dim.XScreenRatio * performance), (int)(dim.YScreenRatio * 80));
            performance_bar_a.ImageRect = new IntRect(0, 75, performance, 140);

            suspensions_bar_a = new Sprite();
            suspensions_bar.AddChild(suspensions_bar_a);
            suspensions_bar_a.Texture = green_bars;
            suspensions_bar_a.SetSize((int)(dim.XScreenRatio * suspensions), (int)(dim.YScreenRatio * 80));
            suspensions_bar_a.SetPosition((int)(dim.XScreenRatio * 90), (int)(dim.YScreenRatio * 16));
            suspensions_bar_a.ImageRect = new IntRect(0, 75, suspensions, 140);

            wheel_bar_a = new Sprite();
            wheel_bar.AddChild(wheel_bar_a);
            wheel_bar_a.Texture = green_bars;
            wheel_bar_a.SetSize((int)(dim.XScreenRatio * wheels), (int)(dim.YScreenRatio * 80));
            wheel_bar_a.SetPosition((int)(dim.XScreenRatio * 90), (int)(dim.YScreenRatio * 16));
            wheel_bar_a.ImageRect = new IntRect(0, 75, wheels, 140);

            brake_bar_a = new Sprite();
            brake_bar.AddChild(brake_bar_a);
            brake_bar_a.Texture = green_bars;
            brake_bar_a.SetSize((int)(dim.XScreenRatio * brake), (int)(dim.YScreenRatio * 80));
            brake_bar_a.SetPosition((int)(dim.XScreenRatio * 90), (int)(dim.YScreenRatio * 16));
            brake_bar_a.ImageRect = new IntRect(0, 75, brake, 140);
        }

        void CreateVehicleBar() {
            var cont_base = cache.GetTexture2D("Textures/Garage/cont_base.png");
            var vehicle = cache.GetTexture2D("Textures/Garage/vehicles2.png");
            var garage_bts = cache.GetTexture2D("Textures/Garage/garage_bts.png");

            // Buttons container (root element)
            cont_downBar = root.CreateSprite();
            cont_downBar.Texture = cont_base;
            cont_downBar.SetSize((int)(dim.XScreenRatio * 1200), (int)(dim.YScreenRatio * 300));
            cont_downBar.SetPosition((int)(dim.XScreenRatio * 10), (int)(dim.YScreenRatio * 700));
            cont_downBar.ImageRect = new IntRect(0, 0, 56, 56);

            // Left arrow - child
            Button btl_left = new Button();
            cont_downBar.AddChild(btl_left);
            btl_left.SetStyleAuto(null);
            btl_left.SetPosition((int)(dim.XScreenRatio * -800), (int)(dim.YScreenRatio * 600));
            btl_left.SetSize((int)(dim.XScreenRatio * 200), (int)(dim.YScreenRatio * 200));
            btl_left.Texture = garage_bts;
            btl_left.ImageRect = new IntRect(560, 420, 730, 600);
            btl_left.Pressed += args => {
                Next_vehicle(0);
            };

            // Vehicle image - root element
            img_vehicle = root.CreateSprite();
            img_vehicle.Texture = vehicle;
            JsonReaderVehicles.SelectSingleVehicle(vehicle_id);
            img_vehicle.SetSize((int)(dim.XScreenRatio * 600), (int)(dim.YScreenRatio * 600));
            img_vehicle.SetPosition((int)(dim.XScreenRatio * 200), (int)(dim.YScreenRatio * 200));

            // Vehicle locked symbol - root element
            selectable = root.CreateSprite();
            root.AddChild(selectable);
            selectable.Texture = garage_bts;
            selectable.SetPosition((int)(dim.XScreenRatio * 480), (int)(dim.YScreenRatio * 830));
            selectable.SetSize((int)(dim.XScreenRatio * 150), (int)(dim.YScreenRatio * 150));
                selectable.ImageRect = new IntRect(110, 295, 205, 405);

            // Right arrow - child
            Button btl_right = new Button();
            cont_downBar.AddChild(btl_right);
            btl_right.SetStyleAuto(null);
            btl_right.SetPosition((int)(dim.XScreenRatio * 700), (int)(dim.YScreenRatio * 600));
            btl_right.SetSize((int)(dim.XScreenRatio * 200), (int)(dim.YScreenRatio * 200));
            btl_right.Texture = garage_bts;
            btl_right.ImageRect = new IntRect(730, 420, 900, 600);
            btl_right.Pressed += args => {
                Next_vehicle(1);
            };

            // button for tre vehicle selection - child
            bt_confirm = new Button();
            cont_downBar.AddChild(bt_confirm);
            bt_confirm.SetStyleAuto(null);
            bt_confirm.SetPosition((int)(dim.XScreenRatio * -400), (int)(dim.YScreenRatio * 650));
            bt_confirm.SetSize((int)(dim.XScreenRatio * 1000), (int)(dim.YScreenRatio * 100));
            bt_confirm.ImageRect = new IntRect(0, 215, 625, 295);
            buttonText = new Text();
            bt_confirm.AddChild(buttonText);
            buttonText.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            buttonText.SetPosition((int)(dim.XScreenRatio * 35), 0);
            buttonText.SetFont(font, dim.XScreenRatio * 35);
            // var garage_bts = cache.GetTexture2D("Textures/Garage/garage_bts.png");
            bt_confirm.Texture = garage_bts;
            vehicle_selected = VehicleManager.Instance.SelectedVehicleId;

            if(vehicle_id == vehicle_selected) 
            {
                selectable.ImageRect = new IntRect(380, 295, 480, 405);
                buttonText.Value = "Selected vehicle";
            }
            else if(VehicleManager.Instance.SelectedVehicleModel.UnlockCost == -1) 
            {
                // TODO: unlockable with components 
                buttonText.Value = "Vehicle not available";
            }
            else if(VehicleManager.Instance.SelectedVehicleModel.UnlockCost >= 0)
            {
                if(CharacterManager.Instance.User.Wallet >= VehicleManager.Instance.SelectedVehicleModel.UnlockCost) 
                {
                    selectable.ImageRect = new IntRect(250, 295, 350, 405);
                    buttonText.Value = "Tap to select this vehicle";
                }
                else 
                {
                    selectable.ImageRect = new IntRect(120, 295, 220, 405);
                    buttonText.Value = "Vehicle not available";
                }
            }
            bt_confirm.Pressed += args => 
            {
                VehicleManager.Instance.CurrentGarageVehicleId = vehicle_id;
                VehicleManager.Instance.SelectedVehicleId = vehicle_id;
                JsonReaderVehicles.SelectSingleVehicle(vehicle_id); // Updates selected vehicle model
                System.Diagnostics.Debug.WriteLine("SAVED ID = " + vehicle_id);
                GameInstance.LaunchScene(GameScenesEnumeration.MENU);

                //QuitConfirm("Are you sure you want to unlock this vehicle?");
            };

            //CreateUpgradeBars_old();
            SetUpgrade_old();
            GetCarImg();
        }

      

        void SetUpgrade_old() {
            var green_bars = cache.GetTexture2D("Textures/Garage/green_bars.png");
            int perf = VehicleManager.Instance.SelectedVehicleModel.Performance;
            int whe = VehicleManager.Instance.SelectedVehicleModel.Wheel;
            int susp = VehicleManager.Instance.SelectedVehicleModel.Suspensions;
            int brk = VehicleManager.Instance.SelectedVehicleModel.Brake;
            // pref, whe, sup or brk *20 equals the X Size of the performance bar.
            int performance = perf * 20;
            int wheels = whe * 20;
            int suspensions = susp * 20;
            int brake = brk * 20;

            performance_bar_a = root.CreateSprite();
            performance_bar_a.Texture = green_bars;
            performance_bar_a.SetPosition((int)(dim.XScreenRatio * 1085), (int)(dim.YScreenRatio * 670));
            performance_bar_a.SetSize((int)(dim.XScreenRatio * performance), (int)(dim.YScreenRatio * 72));
            performance_bar_a.ImageRect = new IntRect(0, 75, performance, 140);

            suspensions_bar_a = root.CreateSprite();
            suspensions_bar_a.Texture = green_bars;
            suspensions_bar_a.SetSize((int)(dim.XScreenRatio * suspensions), (int)(dim.YScreenRatio * 72));
            suspensions_bar_a.SetPosition((int)(dim.XScreenRatio * 1085), (int)(dim.YScreenRatio * 370));
            suspensions_bar_a.ImageRect = new IntRect(0, 75, suspensions, 140);
            
            wheel_bar_a = root.CreateSprite();
            wheel_bar_a.Texture = green_bars;
            wheel_bar_a.SetSize((int)(dim.XScreenRatio * wheels), (int)(dim.YScreenRatio * 72));
            wheel_bar_a.SetPosition((int)(dim.XScreenRatio * 1085), (int)(dim.YScreenRatio * 470));
            wheel_bar_a.ImageRect = new IntRect(0, 75, wheels, 140);

            brake_bar_a = root.CreateSprite();
            brake_bar_a.Texture = green_bars;
            brake_bar_a.SetSize((int)(dim.XScreenRatio * brake), (int)(dim.YScreenRatio * 72));
            brake_bar_a.SetPosition((int)(dim.XScreenRatio * 1085), (int)(dim.YScreenRatio * 570));
            brake_bar_a.ImageRect = new IntRect(0, 75, brake, 140);
        }

        void GetCarImg() {

            /*SELECTED VEHICLE*/
            int left = VehicleManager.Instance.SelectedVehicleModel.ImagePosition.Left;
            int top = VehicleManager.Instance.SelectedVehicleModel.ImagePosition.Top;
            int right = VehicleManager.Instance.SelectedVehicleModel.ImagePosition.Right;
            int bottom = VehicleManager.Instance.SelectedVehicleModel.ImagePosition.Bottom;
            SelectedVehicle.ImageRect = new IntRect(left, top, right, bottom);
            CarName.Value = VehicleManager.Instance.SelectedVehicleModel.Name;
            if(VehicleManager.Instance.SelectedVehicleModel.UnlockCost == -1) {
                screen_info.Value = "Collect components to unlock this vehicle";
                cont_upgrade.Visible = false;
                cont_components.Visible = true;
            }
            else {

                screen_info.Value = "Tap to unlock this vehicle";
                cont_upgrade.Visible = true;
                cont_components.Visible = false;
            }

            if(vehicle_id == vehicle_selected && vehicle_id != -1) {
                System.Diagnostics.Debug.WriteLine(vehicle_id + " + " + vehicle_selected);
                screen_info.Value = "Selected vehicle.";
            }
            else if(CharacterManager.Instance.User.Wallet >= VehicleManager.Instance.SelectedVehicleModel.UnlockCost) {
                if(VehicleManager.Instance.SelectedVehicleModel.UnlockCost == -1) 
                {
                    screen_info.Value = "Collect components to unlock this vehicle";
                    cont_upgrade.Visible = false;
                    cont_components.Visible = true;
                }
                else
                {
                  
                    screen_info.Value = "Tap to unlock this vehicle";
                    cont_upgrade.Visible = true;
                    cont_components.Visible = false;
                }

            }
            else {
                screen_info.Value = "Vehicle not available";
                cont_upgrade.Visible = true;
                cont_components.Visible = false;
            }

            var green_bars = cache.GetTexture2D("Textures/Garage/green_bars.png");
            int perf = VehicleManager.Instance.SelectedVehicleModel.Performance;
            int whe = VehicleManager.Instance.SelectedVehicleModel.Wheel;
            int susp = VehicleManager.Instance.SelectedVehicleModel.Suspensions;
            int brk = VehicleManager.Instance.SelectedVehicleModel.Brake;

            int performance = perf * 20;
            int wheels = whe * 20;
            int suspensions = susp * 20;
            int brake = brk * 20;
            performance_bar_a.SetSize((int)(dim.XScreenRatio * performance), (int)(dim.YScreenRatio * 72));
            performance_bar_a.ImageRect = new IntRect(0, 75, performance, 140);
            suspensions_bar_a.SetSize((int)(dim.XScreenRatio * suspensions), (int)(dim.YScreenRatio * 72));
            suspensions_bar_a.ImageRect = new IntRect(0, 75, suspensions, 140);
            wheel_bar_a.SetSize((int)(dim.XScreenRatio * wheels), (int)(dim.YScreenRatio * 72));
            wheel_bar_a.ImageRect = new IntRect(0, 75, wheels, 140);
            brake_bar_a.SetSize((int)(dim.XScreenRatio * brake), (int)(dim.YScreenRatio * 72));
            brake_bar_a.ImageRect = new IntRect(0, 75, brake, 140);

            /* PREV VEHICLE */
            int prev;
            if(vehicle_id == 0) {
                prev = 13;
            }
            else {
                prev = vehicle_id - 1;
            }
            JsonReaderVehicles.SelectSingleVehicle(prev);
            int leftP = VehicleManager.Instance.SelectedVehicleModel.ImagePosition.Left;
            int topP = VehicleManager.Instance.SelectedVehicleModel.ImagePosition.Top;
            int rightP = VehicleManager.Instance.SelectedVehicleModel.ImagePosition.Right;
            int bottomP = VehicleManager.Instance.SelectedVehicleModel.ImagePosition.Bottom;
            PrevVehicle.ImageRect = new IntRect(leftP, topP, rightP, bottomP);
            /* NEXT VEHICLE */
            int next;
            if (vehicle_id  == 13) {
                next = 0;
            }
            else {
                next = vehicle_id + 1;
            }
            JsonReaderVehicles.SelectSingleVehicle(next);
            int leftN = VehicleManager.Instance.SelectedVehicleModel.ImagePosition.Left;
            int topN = VehicleManager.Instance.SelectedVehicleModel.ImagePosition.Top;
            int rightN = VehicleManager.Instance.SelectedVehicleModel.ImagePosition.Right;
            int bottomN = VehicleManager.Instance.SelectedVehicleModel.ImagePosition.Bottom;
            NextVehicle.ImageRect = new IntRect(leftN, topN, rightN, bottomN);
        }

        void Next_vehicle(int x) {
            if ( x == 0) { // left arrow
                vehicle_id = vehicle_id <= 0 ? vehicles_count - 1 : vehicle_id - 1;
            }
            else if (x == 1) { // right arrow
                vehicle_id = vehicle_id >= vehicles_count - 1 ? 0 : 1 + vehicle_id;
            }
            JsonReaderVehicles.SelectSingleVehicle(vehicle_id);
            GetCarImg();
        }


        void QuitConfirm(String text) {
            var quitWindow = new Window();
            GameInstance.UI.Root.AddChild(quitWindow);
            GameInstance.UI.SetFocusElement(null);

            // Set Window size and layout settings
            quitWindow.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(0));
            quitWindow.SetSize(GameInstance.ScreenInfo.SetX(1920), GameInstance.ScreenInfo.SetY(1080));
            quitWindow.SetColor(Color.FromHex("#22000000"));
            quitWindow.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            quitWindow.Name = "QuitWindow";

            Sprite windowSprite = new Sprite();
            quitWindow.AddChild(windowSprite);
            windowSprite.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.TopBar.ResourcePath);
            windowSprite.Opacity = 0.75f;
            windowSprite.ImageRect = AssetsCoordinates.Generic.TopBar.Rectangle;
            windowSprite.SetSize((int)(dim.XScreenRatio * 1920), (int)(dim.YScreenRatio * 1080));
            windowSprite.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            windowSprite.SetPosition(0, 0);

            Font font = GameInstance.ResourceCache.GetFont(GameInstance.defaultFont);

            Window rectangle = new Window();
            quitWindow.AddChild(rectangle);
            rectangle.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(0));
            rectangle.SetSize(GameInstance.ScreenInfo.SetX(800), GameInstance.ScreenInfo.SetY(200));
            rectangle.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            rectangle.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            rectangle.ImageRect = AssetsCoordinates.Generic.Boxes.BoxConfirmation;

            Text warningText = GameText.CreateText(rectangle, GameInstance.ScreenInfo, font, 35, 250, 0, HorizontalAlignment.Left, VerticalAlignment.Center, text);
            warningText.Wordwrap = true;
            warningText.SetSize(GameInstance.ScreenInfo.SetX(750 - 270), GameInstance.ScreenInfo.SetY(240));
            warningText.SetColor(Color.White);

            var quitButton = new Button();
            quitButton.SetPosition(GameInstance.ScreenInfo.SetX(-95), GameInstance.ScreenInfo.SetY(180));
            quitButton.SetSize(GameInstance.ScreenInfo.SetX(285), GameInstance.ScreenInfo.SetY(130));
            quitButton.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            quitButton.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            quitButton.ImageRect = AssetsCoordinates.Generic.Boxes.SelectionPositive;
            quitWindow.AddChild(quitButton);

            Text confirmText = GameText.CreateText(quitButton, GameInstance.ScreenInfo, font, 50, 145, -5, HorizontalAlignment.Left, VerticalAlignment.Center, "Yes");
            confirmText.SetColor(Color.White);

            quitButton.Pressed += (PressedEventArgs args) => {
                //CloseGameLevel();
                GameInstance.LaunchScene(GameScenesEnumeration.MENU);
            };

            var continueButton = new Button();
            continueButton.SetPosition(GameInstance.ScreenInfo.SetX(245), GameInstance.ScreenInfo.SetY(180));
            continueButton.SetSize(GameInstance.ScreenInfo.SetX(285), GameInstance.ScreenInfo.SetY(130));
            continueButton.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            continueButton.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            continueButton.ImageRect = AssetsCoordinates.Generic.Boxes.SelectionNegative;
            quitWindow.AddChild(continueButton);

            Text cancelText = GameText.CreateText(continueButton, GameInstance.ScreenInfo, font, 50, 145, -5, HorizontalAlignment.Left, VerticalAlignment.Center, "No");
            cancelText.SetColor(Color.White);


            Action<PressedEventArgs> select = new Action<PressedEventArgs>((PressedEventArgs a) => {
                VehicleManager.Instance.CurrentGarageVehicleId = vehicle_id;
                VehicleManager.Instance.SelectedVehicleId = vehicle_id;
                JsonReaderVehicles.SelectSingleVehicle(vehicle_id); // Updates selected vehicle model
                System.Diagnostics.Debug.WriteLine("SAVED ID = " + vehicle_id);
                
                quitWindow.Visible = false;
                quitWindow.Remove();
                GameInstance.LaunchScene(GameScenesEnumeration.MENU);
            });

            quitButton.Pressed += select;          


        }




    }

}
    

       

