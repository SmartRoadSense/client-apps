using System;
using Urho;
using Urho.Gui;
using Urho.Resources;
using Urho.Urho2D;

namespace SmartRoadSense.Shared
{
    public class SceneMenu : BaseScene {
        Sprite logoSprite;
        Sprite backgroundSprite;
        ScreenInfoRatio dim; //variabile rapporto dimensioni schermo

        public SceneMenu(Game game) : base(game) {

            dim = GameInstance.ScreenInfo;
            if(CharacterManager.Instance.User == null)
                GameInstance.LaunchScene(GameScenesEnumeration.PROFILE);
            else if(VehicleManager.Instance.SelectedVehicleId == -1) 
                GameInstance.LaunchScene(GameScenesEnumeration.GARAGE);
            else 
                CreateUI();
        }

        Button CreateButton(int x, int y, int xSize, int ySize, int lr, int tr, int rr, int br, string text, int action) {
            Font font = GameInstance.ResourceCache.GetFont("Fonts/OpenSans-Bold.ttf");
            // Create the button and center the text onto it
            Button button = new Button();
            GameInstance.UI.Root.AddChild(button);
            button.SetStyleAuto(null);
            button.SetPosition((int)(dim.XScreenRatio * x), (int)(dim.YScreenRatio * y));
            button.SetSize((int)(dim.XScreenRatio * xSize), (int)(dim.YScreenRatio * ySize));

            var btn1Texture = GameInstance.ResourceCache.GetTexture2D("Textures/menutmp.png");
            button.Texture = btn1Texture;
            button.ImageRect = new IntRect(lr, tr, rr, br);
            Text buttonText = new Text();
            button.AddChild(buttonText);
            //button.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Top);
            buttonText.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            buttonText.SetPosition(GameInstance.ScreenInfo.SetX(40), 0);
            buttonText.SetFont(font, dim.XScreenRatio*25);
            buttonText.Value = text;

            button.Pressed += args => {
                switch(action) {
                    case 2:
                        GameInstance.LaunchScene(GameScenesEnumeration.LEVEL_SELECT);
                        break;
                    case 3:
                        GameInstance.LaunchScene(GameScenesEnumeration.GARAGE);
                        break;
                    case 7:
                        GameInstance.LaunchScene(GameScenesEnumeration.PROFILE);
                        break;
                    case 9:
                        GameInstance.LaunchScene(GameScenesEnumeration.SETTINGS);
                        break;
                    case 10:
                        GameInstance.LaunchScene(GameScenesEnumeration.USER_PROFILE);
                        break;
                    case 11:
                        ComingSoon();
                        break;
                }
            };

            return button;
        }

        void CreateLogo() {
            var logoTexture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Logos.Logo.ResourcePath);

            if(logoTexture == null)
                return;

            logoSprite = GameInstance.UI.Root.CreateSprite();
            logoSprite.Texture = logoTexture;
            logoSprite.ImageRect = AssetsCoordinates.Logos.Logo.LogoBT;
            logoSprite.SetSize((int) (dim.XScreenRatio*900), (int) (dim.YScreenRatio*500));

            logoSprite.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            logoSprite.SetPosition((int)(dim.YScreenRatio * 500), (int)(dim.YScreenRatio * 100));
        }

        void CreateTopBar() {
            var path = AssetsCoordinates.Generic.Icons.ResourcePath;

            // BACK
            Button btn_back = new Button();
            GameInstance.UI.Root.AddChild(btn_back);
            btn_back.SetStyleAuto(null);
            btn_back.SetPosition((int)(dim.XScreenRatio * 40), (int)(dim.YScreenRatio * 40));
            btn_back.SetSize((int)(dim.XScreenRatio * 120), (int)(dim.YScreenRatio * 120));
            btn_back.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            btn_back.ImageRect = AssetsCoordinates.Generic.Icons.BntBack;
            btn_back.Pressed += args => {
                // Close game
                GameInstance.Graphics.Close();
                GameInstance.Exit();
            };
        }

        void CreateBackground() 
        {
            var backgroundTexture = GameInstance.ResourceCache.GetTexture2D("Textures/MenuBackground.png");

            if(backgroundTexture == null)
                return;

            backgroundSprite = GameInstance.UI.Root.CreateSprite();
            backgroundSprite.Texture = backgroundTexture;
            //backgroundSprite.ImageRect = new IntRect(0, 0, 950, 475);
            backgroundSprite.SetSize((int)(dim.XScreenRatio * 1920), (int)(dim.YScreenRatio * 1080));

            backgroundSprite.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            backgroundSprite.SetPosition(0,0);   
        }

        void CreateUI() {
            CreateBackground();
            CreateTopBar();
            CreateLogo();

            Button button_singleplay = CreateButton(100, 700, 550, 150, 0, 0, 655, 190, "SINGLE PLAYER", 2); // action == GameSceneEnumeration
            Button button_garage = CreateButton(700, 700, 550, 150, 655, 0, 1320, 190, "VEHICLE GARAGE", 3);
            Button button_profile = CreateButton(1300, 700, 550, 150, 1320, 0, 1970, 190, "PROFILE", 10);
            Button button_rewards = CreateButton(100, 880, 550, 150, 0, 190, 655, 380, "REWARDS", 11); 
            Button button_ingame = CreateButton(700, 880, 550, 150, 655, 190, 1320, 380, "IN-GAME STORE", 11);
            Button button_settings = CreateButton(1300, 880, 550, 150, 1320, 190, 1970, 380, "SETTINGS", 9); 

            /*Button button_singleplay = CreateButton(100, 600, 550, 180, 0, 0, 655, 190,  "SINGLE PLAYER", 2); // action == GameSceneEnumeration
            //Button button_multiplayer = CreateButton(150, 600, 800, 80, "CHARACTER PROFILE");
            Button button_garage = CreateButton(700, 600, 550, 180, 655, 0, 1320, 190, "VEHICLE GARAGE", 3);
            Button button_rewards = CreateButton(1300, 600, 550, 180, 1320, 0, 1970, 190, "MULTIPLAYER", 2);
            Button button_ingame = CreateButton(100, 800, 550, 180, 0, 190, 655, 380, "PROFILE", 7); //REWARDS 
            Button button_profile = CreateButton(700, 800, 550, 180, 655, 190, 1320, 380, "IN-GAME STORE", 10);
            Button button_settings = CreateButton(1300, 800, 550, 180, 1320, 190, 1970, 380, "SETTINGS", 9);*/
        }

        void ComingSoon() {
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
            rectangle.UseDerivedOpacity = true; 
            rectangle.SetSize(GameInstance.ScreenInfo.SetX(800), GameInstance.ScreenInfo.SetY(200));
            rectangle.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            rectangle.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            rectangle.ImageRect = AssetsCoordinates.Generic.Boxes.BoxConfirmation;

            Text warningText = GameText.CreateText(rectangle, GameInstance.ScreenInfo, font, 35, 250, 0, HorizontalAlignment.Left, VerticalAlignment.Center, "COMING SOON!");
            warningText.Wordwrap = true;
            warningText.SetSize(GameInstance.ScreenInfo.SetX(750 - 270), GameInstance.ScreenInfo.SetY(240));
            warningText.SetColor(Color.White);

            var continueButton = new Button();
            continueButton.SetPosition(GameInstance.ScreenInfo.SetX(245), GameInstance.ScreenInfo.SetY(180));
            continueButton.SetSize(GameInstance.ScreenInfo.SetX(285), GameInstance.ScreenInfo.SetY(130));
            continueButton.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            continueButton.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            continueButton.ImageRect = AssetsCoordinates.Generic.Boxes.SelectionPositive;
            quitWindow.AddChild(continueButton);

            Text cancelText = GameText.CreateText(continueButton, GameInstance.ScreenInfo, font, 50, 145, -5, HorizontalAlignment.Left, VerticalAlignment.Center, "OK");
            cancelText.SetColor(Color.White);


            Action<PressedEventArgs> select = new Action<PressedEventArgs>((PressedEventArgs a) => {

                quitWindow.Visible = false;
                quitWindow.Remove();
                GameInstance.LaunchScene(GameScenesEnumeration.MENU);
            });

            continueButton.Pressed += select;

        }
    }
}
