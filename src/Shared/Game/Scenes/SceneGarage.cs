using System;
using Urho;
using Urho.Gui;
using Urho.Resources;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace SmartRoadSense.Shared {
    public class SceneGarage : BaseScene {
        readonly ScreenInfoRatio _dim; //variabile rapporto dimensioni schermo
        readonly ResourceCache _cache;
        readonly Font _font;
        readonly UIElement _root;

        // Upgrade bars
        BorderImage _performanceBar;
        Sprite _performanceBarA;
        BorderImage _suspensionsBar;
        Sprite _suspensionsBarA;
        BorderImage _brakeBar;
        Sprite _brakeBarA;
        BorderImage _wheelBar;
        Sprite _wheelBarA;
        //

        // Components
        BorderImage Body;
        BorderImage Wheel;
        BorderImage Suspensions;
        BorderImage Engine;
        //

        Text _screenInfo;
        Text _carName;
        Text _unlockCost;
        Text _wallet;

        Button _selectedVehicle;
        Button _nextVehicle;
        Button _prevVehicle;

        Sprite _lockedVehicle;
        Sprite _contComponents;
        Sprite _blackBar;
        Sprite _contUpgrade;
        Sprite _backgroundSprite;
        BorderImage _coinIcon;

        VehicleModel _currentVehicleModel;
        VehicleModel _lastSelectedVehicleModel;
        int _idDVehicle;

        public SceneGarage(Game game) : base(game) {
            _idDVehicle = VehicleManager.Instance.SelectedVehicleModel != null ? VehicleManager.Instance.SelectedVehicleModel.IdVehicle : -1;
            if(_idDVehicle < 0)
                _idDVehicle = 0;

            _dim = GameInstance.ScreenInfo;
            _root = GameInstance.UI.Root;
            _cache = GameInstance.ResourceCache;
            _font = _cache.GetFont("Fonts/OpenSans-Bold.ttf");
            JsonReaderVehicles.GetVehicleConfig();

            _currentVehicleModel = VehicleManager.Instance.SelectedVehicleModel;
            _lastSelectedVehicleModel = _currentVehicleModel;

            CreateUI();
        }

        private void CreateUI() {
            CreateBackground();
            CreateTopBar();
            CreateVehicleBar();
        }

        void CreateBackground() {
            //TODO: animated background
            var backgroundTexture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Backgrounds.FixedBackground.ResourcePath);
            if(backgroundTexture == null)
                return;
            _backgroundSprite = _root.CreateSprite();
            _backgroundSprite.Texture = backgroundTexture;
            _backgroundSprite.SetSize((int)(_dim.XScreenRatio * 1920), (int)(_dim.YScreenRatio * 1080));
            _backgroundSprite.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            _backgroundSprite.SetPosition(0, 0);
        }

        void CreateTopBar() {

            _blackBar = _root.CreateSprite();
            _root.AddChild(_blackBar);
            _blackBar.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.TopBar.ResourcePath);
            _blackBar.Opacity = 0.5f;
            _blackBar.SetPosition(0, (int)(_dim.YScreenRatio * 30));
            _blackBar.SetSize((int)(_dim.XScreenRatio * 2000), (int)(_dim.YScreenRatio * 140));
            _blackBar.ImageRect = AssetsCoordinates.Generic.TopBar.Rectangle;

            Button btnBack = new Button();
            _blackBar.AddChild(btnBack);
            btnBack.UseDerivedOpacity = false;
            btnBack.SetStyleAuto(null);
            btnBack.SetPosition(_dim.SetX(10), _dim.SetY(0));
            btnBack.HorizontalAlignment = HorizontalAlignment.Left;
            btnBack.VerticalAlignment = VerticalAlignment.Center;
            btnBack.SetSize(_dim.SetX(120), _dim.SetY(120));
            btnBack.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            btnBack.ImageRect = AssetsCoordinates.Generic.Icons.BntBack;
            btnBack.Pressed += args => {
                GameInstance.LaunchScene(GameScenesEnumeration.MENU);
            };

            //COINS
            Button coins = new Button();
            _blackBar.AddChild(coins);
            coins.UseDerivedOpacity = false; 
            coins.SetStyleAuto(null);
            coins.SetPosition(_dim.SetX(150), _dim.SetY(0));
            coins.HorizontalAlignment = HorizontalAlignment.Left;
            coins.VerticalAlignment = VerticalAlignment.Center;
            coins.SetSize(_dim.SetX(70), _dim.SetY(70));
            coins.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            coins.ImageRect = AssetsCoordinates.Generic.Icons.IconCoin;
            coins.UseDerivedOpacity = false;

            //Wallet text
            _wallet = new Text();
            coins.AddChild(_wallet);
            _wallet.SetPosition((int)(_dim.XScreenRatio * 90), (int)(_dim.YScreenRatio * 10));
            _wallet.SetFont(_font, _dim.XScreenRatio * 30);
            _wallet.Value = ""+ CharacterManager.Instance.Wallet;

            // SCREEN TITLE
            Button screen_title = new Button();
            _root.AddChild(screen_title);
            screen_title.SetStyleAuto(null);
            screen_title.SetPosition((int)(_dim.XScreenRatio * 1500), (int)(_dim.YScreenRatio * 50));
            screen_title.SetSize((int)(_dim.XScreenRatio * 400), (int)(_dim.YScreenRatio * 100));
            screen_title.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            screen_title.ImageRect = AssetsCoordinates.Generic.Boxes.BoxTitle;
            screen_title.Enabled = false;

            Text buttonTitleText = new Text();
            screen_title.AddChild(buttonTitleText);
            buttonTitleText.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            buttonTitleText.SetPosition(0, 0);
            buttonTitleText.SetFont(_font, _dim.XScreenRatio * 30);
            buttonTitleText.Value = "VEHICLE SELECT";

            _screenInfo = new Text();
            _root.AddChild(_screenInfo);
            _screenInfo.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Top);
            _screenInfo.SetPosition(_dim.SetX(0), _dim.SetY(220));
            _screenInfo.SetFont(_font, _dim.SetX(40));
            _screenInfo.SetColor(Color.White);
            if(VehicleManager.Instance.UnlockedVehicles == null || VehicleManager.Instance.UnlockedVehicles.VehicleModel.Count == 0) {
                _screenInfo.Value = "Please select a free starting vehicle.";
            }
            else if(_currentVehicleModel.IdVehicle == _idDVehicle) {
                _screenInfo.Value = "";
                //screen_info.Value = "Selected vehicle.";
            }
            else {
                _screenInfo.Value = "";
                //screen_info.Value = "Tap to unlock this vehicle";
            }
        }

        void CreateVehicleBar() {
            var vehicleBackground = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Garage.VehicleBackgroundBar.Path);
            var vehicle = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Garage.FullVehicles.ResourcePath);

            Sprite VehicleBar = _root.CreateSprite();
            VehicleBar.Texture = vehicleBackground;
            VehicleBar.SetSize(_dim.SetX(1920), _dim.SetY(480));
            VehicleBar.SetPosition(_dim.SetX(0), _dim.SetY(330));
            VehicleBar.ImageRect = AssetsCoordinates.Generic.Garage.VehicleBackgroundBar.Rectangle;
            VehicleBar.UseDerivedOpacity = false;
            VehicleBar.Opacity = 0.75f;
            VehicleBar.VerticalAlignment = VerticalAlignment.Top;

            _prevVehicle = new Button();
            VehicleBar.AddChild(_prevVehicle);
            _prevVehicle.Texture = vehicle;
            _prevVehicle.Opacity = 0.7f;
            _prevVehicle.UseDerivedOpacity = false;
            _prevVehicle.SetSize((int)(_dim.XScreenRatio * 450), (int)(_dim.YScreenRatio * 450));
            _prevVehicle.SetPosition((int)(_dim.XScreenRatio * 0), (int)(_dim.YScreenRatio * 0));
            _prevVehicle.Pressed += args => {
                Next_vehicle(0);
            };

            // Selected Vehicle image - root element
            _selectedVehicle = new Button();
            VehicleBar.AddChild(_selectedVehicle);
            _selectedVehicle.UseDerivedOpacity = false;
            _selectedVehicle.Texture = vehicle;
            _currentVehicleModel = VehicleManager.Instance.GetVehicleFromId(_idDVehicle);
            _selectedVehicle.SetSize((int)(_dim.XScreenRatio * 600), (int)(_dim.YScreenRatio * 600));
            _selectedVehicle.SetPosition((int)(_dim.XScreenRatio * 650), (int)(_dim.YScreenRatio * -100));

            _selectedVehicle.Pressed += args => {
                if(VehicleManager.Instance.UnlockedVehicles.VehicleModel.Count == 0) 
                {
                    if(_currentVehicleModel.UnlockCost == -1) 
                    {
                        ConfirmationWindow(string.Format("You can't select this vehicle at this moment."), true);
                    }
                    else 
                    {
                        // Unlock vehicle
                        VehicleManager.Instance.SelectedVehicleModel = VehicleManager.Instance.Vehicles.VehicleModel.First(v => v.IdVehicle == _idDVehicle);
                        VehicleManager.Instance.UnlockVehicle();

                        Debug.WriteLine("Unlocked vehicle " + _currentVehicleModel.IdVehicle);
                        ConfirmationWindow(string.Format("{0} unlocked!", _selectedVehicle.Name), false, true);
                    }
                }
                else if (!VehicleManager.Instance.UnlockedVehicles.VehicleModel.Exists(v => v.IdVehicle == _currentVehicleModel.IdVehicle)) 
                {
                    // If selected and vehicle needs to be unlocked, unlock vehicle without changing scene
                    if(_currentVehicleModel.UnlockCost != -1) {
                        if(CharacterManager.Instance.Wallet >= _currentVehicleModel.UnlockCost) {
                            CharacterManager.Instance.Wallet -= _currentVehicleModel.UnlockCost;
                            _wallet.Value = CharacterManager.Instance.Wallet.ToString();
                            // Unlock vehicle
                            VehicleManager.Instance.SelectedVehicleModel = VehicleManager.Instance.Vehicles.VehicleModel.First(v => v.IdVehicle == _idDVehicle);
                            VehicleManager.Instance.UnlockVehicle();

                            Debug.WriteLine("Unlocked vehicle " + _currentVehicleModel.IdVehicle);
                            ConfirmationWindow(string.Format("{0} unlocked!", _selectedVehicle.Name), false, true);
                        }
                        else {
                            ConfirmationWindow(string.Format("Collect more coins to unlock this vehicle."), true);
                        }
                    }
                    else {
                        var components = VehicleManager.Instance.CollectedComponentsForVehicle(_currentVehicleModel.IdVehicle).VehicleComponents;
                        if(components.Brakes && components.Performance && components.Suspensions && components.Wheels) {
                            // Unlock vehicle
                            VehicleManager.Instance.SelectedVehicleModel = VehicleManager.Instance.Vehicles.VehicleModel.First(v => v.IdVehicle == _idDVehicle);
                            VehicleManager.Instance.UnlockVehicle();

                            // TODO: update UI
                            Debug.WriteLine("Unlocked vehicle " + _currentVehicleModel.IdVehicle);
                            ConfirmationWindow(string.Format(_selectedVehicle.Name + "unlocked!"), false, true);
                        }
                        else 
                        {
                            ConfirmationWindow(string.Format("Collect all four components to unlock this vehicle."), true);
                        }
                    }
                }
                else 
                {
                    VehicleManager.Instance.SelectedVehicleModel = VehicleManager.Instance.Vehicles.VehicleModel.First(v => v.IdVehicle == _idDVehicle);
                    GameInstance.LaunchScene(GameScenesEnumeration.MENU);
                }
            };

            _carName = new Text();
            _selectedVehicle.AddChild(_carName);
            _carName.SetPosition(_dim.SetX(0), _dim.SetY(-80));
            _carName.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Bottom);
            _carName.SetFont(_font, _dim.SetX(40));

            _unlockCost = new Text();
            _selectedVehicle.AddChild(_unlockCost);
            _unlockCost.SetPosition(_dim.SetX(40), _dim.SetY(-30));
            _unlockCost.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Bottom);
            _unlockCost.SetFont(_font, _dim.SetX(30));
            _unlockCost.Visible = false;

            _coinIcon = new BorderImage();
            _selectedVehicle.AddChild(_coinIcon);
            _coinIcon.SetPosition(_dim.SetX(-50), _dim.SetY(-30));
            _coinIcon.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Bottom);
            _coinIcon.Visible = false;
            _coinIcon.SetSize(_dim.SetX(50), _dim.SetY(50));
            _coinIcon.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            _coinIcon.ImageRect = AssetsCoordinates.Generic.Icons.IconCoin;
            _coinIcon.UseDerivedOpacity = false;

            _lockedVehicle = new Sprite();
            _selectedVehicle.AddChild(_lockedVehicle);
            _lockedVehicle.SetPosition((int)(_dim.XScreenRatio * 630), (int)(_dim.YScreenRatio * 200));
            _lockedVehicle.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            _lockedVehicle.ImageRect = AssetsCoordinates.Generic.Icons.IconLocked;
            _lockedVehicle.SetSize(_dim.SetX(100), _dim.SetY(100));

            _nextVehicle = new Button();
            VehicleBar.AddChild(_nextVehicle);
            _nextVehicle.UseDerivedOpacity = false;
            _nextVehicle.Texture = vehicle;
            _nextVehicle.Opacity = 0.7f;
            _nextVehicle.SetSize((int)(_dim.XScreenRatio * 450), (int)(_dim.YScreenRatio * 450));
            _nextVehicle.SetPosition((int)(_dim.XScreenRatio * 1400), (int)(_dim.YScreenRatio * 0));
            _nextVehicle.Pressed += args => {
                Next_vehicle(1);
            };

            CreateUpgradeBars();
        }

        void CreateUpgradeBars() {
            var contBase = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Garage.ContBase.ResourcePath);

            // UPDGRADES
            _contUpgrade = _root.CreateSprite();
            _contUpgrade.Texture = contBase;
            _contUpgrade.SetSize((int)(_dim.XScreenRatio * 1200), (int)(_dim.YScreenRatio * 300));
            _contUpgrade.SetPosition((int)(_dim.XScreenRatio * 0), (int)(_dim.YScreenRatio * 750));
            _contUpgrade.ImageRect = AssetsCoordinates.Generic.Garage.ContBase.TrasparentItem;

            _performanceBar = new BorderImage();
            _contUpgrade.AddChild(_performanceBar);
            _performanceBar.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            _performanceBar.ImageRect = AssetsCoordinates.Generic.Boxes.BoxPerformanceUpgrade;
            _performanceBar.SetSize((int)(_dim.XScreenRatio * 550), (int)(_dim.YScreenRatio * 95));
            _performanceBar.SetPosition((int)(_dim.XScreenRatio * 1000), (int)(_dim.YScreenRatio * 100));

            _suspensionsBar = new BorderImage();
            _contUpgrade.AddChild(_suspensionsBar);
            _suspensionsBar.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            _suspensionsBar.ImageRect = AssetsCoordinates.Generic.Boxes.BoxSuspensionUpgrade;
            _suspensionsBar.SetSize((int)(_dim.XScreenRatio * 550), (int)(_dim.YScreenRatio * 95));
            _suspensionsBar.SetPosition((int)(_dim.XScreenRatio * 1000), (int)(_dim.YScreenRatio * 200));

            _wheelBar = new BorderImage();
            _contUpgrade.AddChild(_wheelBar);
            _wheelBar.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            _wheelBar.ImageRect = AssetsCoordinates.Generic.Boxes.BoxWheelUpgrade;
            _wheelBar.SetSize((int)(_dim.XScreenRatio * 550), (int)(_dim.YScreenRatio * 95));
            _wheelBar.SetPosition((int)(_dim.XScreenRatio * 440), (int)(_dim.YScreenRatio * 100));

            _brakeBar = new BorderImage();
            _contUpgrade.AddChild(_brakeBar);
            _brakeBar.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            _brakeBar.ImageRect = AssetsCoordinates.Generic.Boxes.BoxBrakeUpgrade;
            _brakeBar.SetSize((int)(_dim.XScreenRatio * 550), (int)(_dim.YScreenRatio * 95));
            _brakeBar.SetPosition((int)(_dim.XScreenRatio * 440), (int)(_dim.YScreenRatio * 200));

            // COMPONENTS
            _contComponents = _root.CreateSprite();
            _contComponents.Texture = contBase;
            _contComponents.SetSize(_dim.SetX(1920), _dim.SetY(300));
            _contComponents.SetPosition(_dim.SetX(0), _dim.SetY(720));
            _contComponents.ImageRect = new IntRect(0, 0, 56, 56);

            Body = new BorderImage();
            _contComponents.AddChild(Body);
            Body.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            Body.ImageRect = AssetsCoordinates.Generic.Boxes.ComponentBodyRed;
            Body.SetSize(_dim.SetX(150), _dim.SetY(150));
            Body.SetPosition(_dim.SetX(-300), _dim.SetY(150));
            Body.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Top);

            Engine = new BorderImage();
            _contComponents.AddChild(Engine);
            Engine.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            Engine.ImageRect = AssetsCoordinates.Generic.Boxes.ComponentEngineRed;
            Engine.SetSize(_dim.SetX(150), _dim.SetY(150));
            Engine.SetPosition(_dim.SetX(-130), _dim.SetY(150));
            Engine.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Top);

            Suspensions = new BorderImage();
            _contComponents.AddChild(Suspensions);
            Suspensions.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            Suspensions.ImageRect = AssetsCoordinates.Generic.Boxes.ComponentSuspensionRed;
            Suspensions.SetSize(_dim.SetX(150), _dim.SetY(150));
            Suspensions.SetPosition(_dim.SetX(130), _dim.SetY(150));
            Suspensions.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Top);

            Wheel = new BorderImage();
            _contComponents.AddChild(Wheel);
            Wheel.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            Wheel.ImageRect = AssetsCoordinates.Generic.Boxes.ComponentWheelRed;
            Wheel.SetSize(_dim.SetX(150), _dim.SetY(150));
            Wheel.SetPosition(_dim.SetX(300), _dim.SetY(150));
            Wheel.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Top);

            SetUpgrade();
            SetCollectedComponents();
            GetCarImg();
        }


        void SetCollectedComponents() {
            if(_currentVehicleModel.UnlockCost == -1) {
                
                if (VehicleManager.Instance.CollectedComponents.CollectedComponentsList != null) {
                    foreach(var v in VehicleManager.Instance.CollectedComponents.CollectedComponentsList) {
                        if(v.VehicleId == _currentVehicleModel.IdVehicle) {
                            //v.VehicleComponents.Suspensions = true;

                            if (v.VehicleComponents.Suspensions) {
                                Suspensions.ImageRect = AssetsCoordinates.Generic.Boxes.ComponentSuspensionGreen;
                            }
                            if(v.VehicleComponents.Wheels) {
                                Wheel.ImageRect = AssetsCoordinates.Generic.Boxes.ComponentWheelGreen;
                            }
                            if(v.VehicleComponents.Performance) {
                                Engine.ImageRect = AssetsCoordinates.Generic.Boxes.ComponentEngineGreen;
                            }
                            if(v.VehicleComponents.Brakes) {
                                Body.ImageRect = AssetsCoordinates.Generic.Boxes.ComponentBodyGreen;
                            }
                        }
                    }
                }
            }
        }

        void SetUpgrade() {
            var green_bars = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Garage.GreenBars.ResourcePath);

            var selectedVehicle = _currentVehicleModel;
            if(VehicleManager.Instance.UnlockedVehicles.VehicleModel.Contains(selectedVehicle))
                selectedVehicle = VehicleManager.Instance.UnlockedVehicles.VehicleModel.First(v => v.IdVehicle == _currentVehicleModel.IdVehicle);

            int perf = selectedVehicle.Performance;
            int whe = selectedVehicle.Wheel;
            int susp = selectedVehicle.Suspensions;
            int brk = selectedVehicle.Brake;

            // perf, whe, sup or brk *20 equals the X Size of the performance bar.
            int performance = perf * 20;
            int wheels = whe * 20;
            int suspensions = susp * 20;
            int brake = brk * 20;

            _performanceBarA = new Sprite(); 
            _performanceBar.AddChild(_performanceBarA);
            _performanceBarA.Texture = green_bars;
            _performanceBarA.SetPosition((int)(_dim.XScreenRatio * 90), (int)(_dim.YScreenRatio * 16));
            _performanceBarA.SetSize((int)(_dim.XScreenRatio * performance), (int)(_dim.YScreenRatio * 80));
            _performanceBarA.ImageRect = new IntRect(0, 75, performance, 140);

            _suspensionsBarA = new Sprite();
            _suspensionsBar.AddChild(_suspensionsBarA);
            _suspensionsBarA.Texture = green_bars;
            _suspensionsBarA.SetSize((int)(_dim.XScreenRatio * suspensions), (int)(_dim.YScreenRatio * 80));
            _suspensionsBarA.SetPosition((int)(_dim.XScreenRatio * 90), (int)(_dim.YScreenRatio * 16));
            _suspensionsBarA.ImageRect = new IntRect(0, 75, suspensions, 140);

            _wheelBarA = new Sprite();
            _wheelBar.AddChild(_wheelBarA);
            _wheelBarA.Texture = green_bars;
            _wheelBarA.SetSize((int)(_dim.XScreenRatio * wheels), (int)(_dim.YScreenRatio * 80));
            _wheelBarA.SetPosition((int)(_dim.XScreenRatio * 90), (int)(_dim.YScreenRatio * 16));
            _wheelBarA.ImageRect = new IntRect(0, 75, wheels, 140);

            _brakeBarA = new Sprite();
            _brakeBar.AddChild(_brakeBarA);
            _brakeBarA.Texture = green_bars;
            _brakeBarA.SetSize((int)(_dim.XScreenRatio * brake), (int)(_dim.YScreenRatio * 80));
            _brakeBarA.SetPosition((int)(_dim.XScreenRatio * 90), (int)(_dim.YScreenRatio * 16));
            _brakeBarA.ImageRect = new IntRect(0, 75, brake, 140);
        }

        void GetCarImg() {
            /*SELECTED VEHICLE*/
            int left = _currentVehicleModel.ImagePosition.Left;
            int top = _currentVehicleModel.ImagePosition.Top;
            int right = _currentVehicleModel.ImagePosition.Right;
            int bottom = _currentVehicleModel.ImagePosition.Bottom;
            _selectedVehicle.ImageRect = new IntRect(left, top, right, bottom);
            _carName.Value = _currentVehicleModel.Name;
            _unlockCost.Value = string.Format($"{_currentVehicleModel.UnlockCost}");
            _unlockCost.SetColor(Color.FromHex("#B29600"));

            switch(VehicleManager.Instance.UnlockedVehicles.VehicleModel.Count) {
                // New game - no vehicles selected
                case 0:
                    // STARTING VEHICLE LOGIC
                    if(_currentVehicleModel.UnlockCost > -1) {
                        _screenInfo.Value = "Tap to unlock this vehicle";
                        _contUpgrade.Visible = false;
                        _contComponents.Visible = false;
                        _unlockCost.Visible = false;
                        _coinIcon.Visible = false;
                    } 
                    else 
                    {
                        _screenInfo.Value = "Vehicle not available";
                        _contUpgrade.Visible = false;
                        _contComponents.Visible = false;
                        _unlockCost.Visible = false;
                        _coinIcon.Visible = false;
                    }
                    break;
                // Returning player - has at least one vehicle
                default:
                    // GAME VEHICLE SELECT LOGIC
                    if(_lastSelectedVehicleModel != null && _idDVehicle == _lastSelectedVehicleModel.IdVehicle && _idDVehicle != -1) 
                    {
                        Debug.WriteLine(_currentVehicleModel.IdVehicle);
                        _screenInfo.Value = "Selected vehicle";
                        _contUpgrade.Visible = true;
                        _contComponents.Visible = false;
                        _unlockCost.Visible = false;
                        _coinIcon.Visible = false;
                    }
                    else 
                    {
                        if(VehicleManager.Instance.UnlockedVehicles.VehicleModel.Exists(v => v.IdVehicle == _currentVehicleModel.IdVehicle)) 
                        {
                            _screenInfo.Value = "Tap to select this vehicle";
                            _contUpgrade.Visible = true;
                            _contComponents.Visible = false;
                            _unlockCost.Visible = false;
                            _coinIcon.Visible = false;
                        }
                        else if (CharacterManager.Instance.User.Wallet >= _currentVehicleModel.UnlockCost) 
                        {
                            if(_currentVehicleModel.UnlockCost == -1) 
                            {
                                _screenInfo.Value = "Collect all components to unlock this vehicle";
                                _contUpgrade.Visible = false;
                                _contComponents.Visible = true;
                                _unlockCost.Visible = false;
                                _coinIcon.Visible = false;
                            }
                            else 
                            {
                                _screenInfo.Value = "Tap to unlock this vehicle";
                                _contUpgrade.Visible = false;
                                _contComponents.Visible = false;
                                _unlockCost.Visible = true;
                                _coinIcon.Visible = true;
                            }
                        }
                        else 
                        {
                            _screenInfo.Value = "Vehicle not available";
                            _contUpgrade.Visible = true;
                            _contComponents.Visible = false;
                            _unlockCost.Visible = true;
                            _coinIcon.Visible = true;
                        }
                    }
                    break;
            }

            var selectedVehicle = _currentVehicleModel;
            
            if(VehicleManager.Instance.UnlockedVehicles.VehicleModel.Contains(selectedVehicle))
                selectedVehicle = VehicleManager.Instance.UnlockedVehicles.VehicleModel.First(v => v.IdVehicle == _currentVehicleModel.IdVehicle);
                
            int perf = selectedVehicle.Performance;
            int whe = selectedVehicle.Wheel;
            int susp = selectedVehicle.Suspensions;
            int brk = selectedVehicle.Brake;

            int performance = perf * 20;
            int wheels = whe * 20;
            int suspensions = susp * 20;
            int brake = brk * 20;

            _performanceBarA.SetSize((int)(_dim.XScreenRatio * performance), (int)(_dim.YScreenRatio * 72));
            _performanceBarA.ImageRect = new IntRect(0, 75, performance, 140);
            _suspensionsBarA.SetSize((int)(_dim.XScreenRatio * suspensions), (int)(_dim.YScreenRatio * 72));
            _suspensionsBarA.ImageRect = new IntRect(0, 75, suspensions, 140);
            _wheelBarA.SetSize((int)(_dim.XScreenRatio * wheels), (int)(_dim.YScreenRatio * 72));
            _wheelBarA.ImageRect = new IntRect(0, 75, wheels, 140);
            _brakeBarA.SetSize((int)(_dim.XScreenRatio * brake), (int)(_dim.YScreenRatio * 72));
            _brakeBarA.ImageRect = new IntRect(0, 75, brake, 140);

            /* PREV VEHICLE */
            int prev;
            if(_currentVehicleModel.IdVehicle == 0) {
                prev = VehicleManager.Instance.VehicleCount - 1;
            }
            else {
                prev = _currentVehicleModel.IdVehicle - 1;
            }

            var _prevVehicleModel = VehicleManager.Instance.GetVehicleFromId(prev);
            int leftP = _prevVehicleModel.ImagePosition.Left;
            int topP = _prevVehicleModel.ImagePosition.Top;
            int rightP = _prevVehicleModel.ImagePosition.Right;
            int bottomP = _prevVehicleModel.ImagePosition.Bottom;
            _prevVehicle.ImageRect = new IntRect(leftP, topP, rightP, bottomP);

            /* NEXT VEHICLE */
            int next;
            if (_currentVehicleModel.IdVehicle == VehicleManager.Instance.VehicleCount-1) {
                next = 0;
            }
            else {
                next = 1 + _currentVehicleModel.IdVehicle;
            }

            var nextVehicle = VehicleManager.Instance.GetVehicleFromId(next);
            int leftN = nextVehicle.ImagePosition.Left;
            int topN = nextVehicle.ImagePosition.Top;
            int rightN = nextVehicle.ImagePosition.Right;
            int bottomN = nextVehicle.ImagePosition.Bottom;
            _nextVehicle.ImageRect = new IntRect(leftN, topN, rightN, bottomN);

            LockedVehicle();
        }

        void Next_vehicle(int x) {
            if ( x == 0) { // left arrow
                _idDVehicle = _idDVehicle <= 0 ? VehicleManager.Instance.VehicleCount - 1 : _idDVehicle - 1;
            }
            else if (x == 1) { // right arrow
                _idDVehicle = _idDVehicle >= VehicleManager.Instance.VehicleCount - 1 ? 0 : 1 + _idDVehicle;
            }
            _currentVehicleModel = VehicleManager.Instance.GetVehicleFromId(_idDVehicle);
            GetCarImg();
        }

        void LockedVehicle() {
            if(VehicleManager.Instance.UnlockedVehicles.VehicleModel.Exists(v => v.IdVehicle == _currentVehicleModel.IdVehicle)) 
            {
                _lockedVehicle.Visible = false;
            }
            else 
            {
                _lockedVehicle.Visible = true;
            }
        }

        void ConfirmationWindow(string text, bool unavailableVehicle, bool reloadGarage = false) 
        {
            var window = new Window();
            GameInstance.UI.Root.AddChild(window);

            // Set Window size and layout settings
            window.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(0));
            window.SetSize(GameInstance.ScreenInfo.SetX(1920), GameInstance.ScreenInfo.SetY(1080));
            window.SetColor(Color.FromHex("#22000000"));
            window.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            window.Name = "ConfirmationWindow";

            Sprite windowSprite = new Sprite();
            window.AddChild(windowSprite);
            windowSprite.Name = "confirmationWindowSprite";
            windowSprite.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.TopBar.ResourcePath);
            windowSprite.Opacity = 0.75f;
            windowSprite.ImageRect = AssetsCoordinates.Generic.TopBar.Rectangle;
            windowSprite.SetSize((int)(_dim.XScreenRatio * 1920), (int)(_dim.YScreenRatio * 1080));
            windowSprite.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            windowSprite.SetPosition(0, 0);

            Window rectangle = new Window();
            window.AddChild(rectangle);
            rectangle.Name = "confirmationWindowRectangle";
            rectangle.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(0));
            rectangle.SetSize(GameInstance.ScreenInfo.SetX(800), GameInstance.ScreenInfo.SetY(200));
            rectangle.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            rectangle.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            rectangle.ImageRect = AssetsCoordinates.Generic.Boxes.BoxConfirmation;

            Text warningText = GameText.CreateText(rectangle, GameInstance.ScreenInfo, _font, 30, 250, 0, HorizontalAlignment.Left, VerticalAlignment.Center, text);
            warningText.Name = "confirmationWindowText";
            warningText.Wordwrap = true;
            warningText.SetSize(GameInstance.ScreenInfo.SetX(750 - 270), GameInstance.ScreenInfo.SetY(240));
            warningText.SetColor(Color.White);

            var closeButton = new Button();
            window.AddChild(closeButton);
            closeButton.Name = "confirmationWindowButton";
            closeButton.SetPosition(GameInstance.ScreenInfo.SetX(245), GameInstance.ScreenInfo.SetY(180));
            closeButton.SetSize(GameInstance.ScreenInfo.SetX(285), GameInstance.ScreenInfo.SetY(130));
            closeButton.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            closeButton.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            closeButton.ImageRect = AssetsCoordinates.Generic.Boxes.SelectionPositive;

            Text confirmText = GameText.CreateText(closeButton, GameInstance.ScreenInfo, _font, 50, 145, -5, HorizontalAlignment.Left, VerticalAlignment.Center, "Ok");
            confirmText.Name = "confirmationWindowConfirmText";
            confirmText.SetColor(Color.White);

            closeButton.Pressed += (PressedEventArgs args) => {
                window.Remove();
                if(unavailableVehicle)
                    return;

                if(VehicleManager.Instance.UnlockedVehicles.VehicleModel.Count == 1) {
                    GameInstance.LaunchScene(GameScenesEnumeration.MENU);
                    return;
                }
                if(reloadGarage)
                    GameInstance.LaunchScene(GameScenesEnumeration.GARAGE);
            };
        }
    }
}
