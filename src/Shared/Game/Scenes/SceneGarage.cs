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
        Sprite performance_bar;
        Sprite performance_bar_a;
        Sprite selectable;
        Sprite suspensions_bar;
        Sprite suspensions_bar_a;
        Sprite cont_downBar;
        Sprite brake_bar;
        Sprite brake_bar_a;
        Text screen_info;
        Sprite wheel_bar;
        Sprite wheel_bar_a;
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
            CreateVehicleBar();
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
            coins.SetPosition((int)(dim.XScreenRatio * 165), (int)(dim.YScreenRatio * 60));
            coins.SetSize((int)(dim.XScreenRatio * 70), (int)(dim.YScreenRatio * 70));
            coins.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            coins.ImageRect = AssetsCoordinates.Generic.Icons.CoinsIcon;
            coins.Visible = false;

            //Wallet text
            Text wallet = new Text();
            root.AddChild(wallet);
            wallet.SetPosition((int)(dim.XScreenRatio * 250), (int)(dim.YScreenRatio * 70));
            wallet.SetFont(font, dim.XScreenRatio * 30);
            int wallet_tot = CharacterManager.Instance.Wallet;
            wallet.Value = ""+ wallet_tot;
            wallet.Visible = false;

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
            JsonReaderVehicles.GetSingleVehicle(vehicle_id);
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
                JsonReaderVehicles.GetSingleVehicle(vehicle_id); // Updates selected vehicle model
                System.Diagnostics.Debug.WriteLine("SAVED ID = " + vehicle_id);
                GameInstance.LaunchScene(GameScenesEnumeration.MENU);

                //QuitConfirm("Are you sure you want to unlock this vehicle?");
            };

            CreateUpgradeBars();
            SetUpgrade();
            GetCarImg();
        }

        void CreateUpgradeBars() {
            var garage_bts = cache.GetTexture2D("Textures/Garage/garage_bts.png");
            var green_bars = cache.GetTexture2D("Textures/Garage/green_bars.png");
            var cont_base = cache.GetTexture2D("Textures/Garage/cont_base.png");
            var vehicle = cache.GetTexture2D("Textures/Garage/vehicles2.png");
            cont_upgrade = root.CreateSprite();
            cont_downBar.Texture = cont_base;
            cont_downBar.SetSize((int)(dim.XScreenRatio * 1200), (int)(dim.YScreenRatio * 300));
            cont_downBar.SetPosition((int)(dim.XScreenRatio * 900), (int)(dim.YScreenRatio * 200));
            cont_downBar.ImageRect = new IntRect(0, 0, 56 , 56);


            upgrademenu = new Sprite();
            cont_upgrade.AddChild(upgrademenu);
            upgrademenu.Texture = garage_bts;
            upgrademenu.SetSize((int)(dim.XScreenRatio * 838), (int)(dim.YScreenRatio * 95));
            upgrademenu.SetPosition((int)(dim.XScreenRatio * 1000), (int)(dim.YScreenRatio * 250));
            upgrademenu.ImageRect = new IntRect(0, 930, 840, 1023);
            upgrademenu.Visible = false;

            performance_bar = new Sprite();
            cont_upgrade.AddChild(performance_bar);
            performance_bar.Texture = garage_bts;
            performance_bar.SetSize((int)(dim.XScreenRatio * 550), (int)(dim.YScreenRatio * 95));
            performance_bar.SetPosition((int)(dim.XScreenRatio * 1000), (int)(dim.YScreenRatio * 650));
            performance_bar.ImageRect = new IntRect(0, 420, 560, 515);

            suspensions_bar = new Sprite();
            cont_upgrade.AddChild(suspensions_bar);
            suspensions_bar.Texture = garage_bts;
            suspensions_bar.SetSize((int)(dim.XScreenRatio * 550), (int)(dim.YScreenRatio * 95));
            suspensions_bar.SetPosition((int)(dim.XScreenRatio * 1000), (int)(dim.YScreenRatio * 350));
            suspensions_bar.ImageRect = new IntRect(0, 515, 560, 610);            

            wheel_bar = new Sprite();
            cont_upgrade.AddChild(wheel_bar);
            wheel_bar.Texture = garage_bts;
            wheel_bar.SetSize((int)(dim.XScreenRatio * 550), (int)(dim.YScreenRatio * 95));
            wheel_bar.SetPosition((int)(dim.XScreenRatio * 1000), (int)(dim.YScreenRatio * 450));
            wheel_bar.ImageRect = new IntRect(0, 610, 560, 705);

            brake_bar = new Sprite();
            cont_upgrade.AddChild(brake_bar);
            brake_bar.Texture = garage_bts;
            brake_bar.SetSize((int)(dim.XScreenRatio * 550), (int)(dim.YScreenRatio * 95));
            brake_bar.SetPosition((int)(dim.XScreenRatio * 1000), (int)(dim.YScreenRatio * 550));
            brake_bar.ImageRect = new IntRect(0, 705, 560, 800);          
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
            int left = VehicleManager.Instance.SelectedVehicleModel.ImagePosition.Left;
            int top = VehicleManager.Instance.SelectedVehicleModel.ImagePosition.Top;
            int right = VehicleManager.Instance.SelectedVehicleModel.ImagePosition.Right;
            int bottom = VehicleManager.Instance.SelectedVehicleModel.ImagePosition.Bottom;

            img_vehicle.ImageRect = new IntRect(left, top, right, bottom);
            if(vehicle_id == vehicle_selected && vehicle_id != -1) {
                bt_confirm.Enabled = true;
                selectable.ImageRect = new IntRect(380, 295, 480, 405);
                buttonText.Value = "Selected vehicle";
                System.Diagnostics.Debug.WriteLine(vehicle_id + " + " + vehicle_selected);
                screen_info.Value = "";
                //screen_info.Value = "Selected vehicle.";
            }
            else if(CharacterManager.Instance.User.Wallet >= VehicleManager.Instance.SelectedVehicleModel.UnlockCost) {
                if(VehicleManager.Instance.SelectedVehicleModel.UnlockCost == -1) 
                {
                    bt_confirm.Enabled = false;
                    selectable.ImageRect = new IntRect(120, 295, 220, 405);
                    buttonText.Value = "Vehicle not available";
                    screen_info.Value = "";
                }
                else
                {
                    bt_confirm.Enabled = true;
                    selectable.ImageRect = new IntRect(250, 295, 350, 405);
                    buttonText.Value = "Tap to select this vehicle";
                    screen_info.Value = "";
                    //screen_info.Value = "Tap to unlock this vehicle";
                }

            }
            else {
                bt_confirm.Enabled = false;
                selectable.ImageRect = new IntRect(120, 295, 220, 405);
                buttonText.Value = "Vehicle not available";
                screen_info.Value = "";
                //screen_info.Value = "Vehicle not available";
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
        }

        void Next_vehicle(int x) {
            if ( x == 0) { // left arrow
                vehicle_id = vehicle_id <= 0 ? vehicles_count - 1 : vehicle_id - 1;
            }
            else if (x == 1) { // right arrow
                vehicle_id = vehicle_id >= vehicles_count - 1 ? 0 : 1 + vehicle_id;
            }
            JsonReaderVehicles.GetSingleVehicle(vehicle_id);
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
                JsonReaderVehicles.GetSingleVehicle(vehicle_id); // Updates selected vehicle model
                System.Diagnostics.Debug.WriteLine("SAVED ID = " + vehicle_id);
                
                quitWindow.Visible = false;
                quitWindow.Remove();
                GameInstance.LaunchScene(GameScenesEnumeration.MENU);
            });

            quitButton.Pressed += select;          


        }


        


    }

}
    

       

