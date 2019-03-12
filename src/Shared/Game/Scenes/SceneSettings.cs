using System;
#if __ANDROID__
using Plugin.CurrentActivity;
using Android.Content;
using Com.Microsoft.Appcenter.Ingestion.Models;
using Android.App;
using SmartRoadSense.Android;
#elif __IOS__
using UIKit;
using Foundation;
#endif
using Urho;
using Urho.Gui;

namespace SmartRoadSense.Shared {
    public class SceneSettings : BaseScene {

        Font _baseFont;
        int ButtonDimensions;
        int ButtonOrientation;
        Text a2;
        Text b2;
        Sprite Buttons;
        Sprite AccBrakeBtn;

        public SceneSettings(Game game) : base(game) {
            _baseFont = GameInstance.ResourceCache.GetFont(GameInstance.defaultFont);
            CreateUI();
        }

        public void CreateUI() {
            ButtonDimensions = CharacterManager.Instance.ButtonDimension;
            ButtonOrientation = CharacterManager.Instance.ButtonOrientation;
            CreateBackground();
            CreateTopBar();
            CreateLeftSection();
            SetButtonConfig();
            CreateRightSection();
        }

        void CreateBackground() {
            //TODO: animated background
            var backgroundTexture = GameInstance.ResourceCache.GetTexture2D("Textures/MenuBackground.png");
            if(backgroundTexture == null)
                return;
            Sprite backgroundSprite = GameInstance.UI.Root.CreateSprite();
            backgroundSprite.Texture = backgroundTexture;
            backgroundSprite.SetSize(GameInstance.ScreenInfo.SetX(1920), GameInstance.ScreenInfo.SetY(1080));
            backgroundSprite.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            backgroundSprite.SetPosition(0, 0);
        }

        void CreateTopBar() {
            var black_bar = GameInstance.UI.Root.CreateSprite();
            GameInstance.UI.Root.AddChild(black_bar);
            black_bar.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.TopBar.ResourcePath);
            black_bar.Opacity = 0.5f;
            black_bar.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(30));
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
            GameInstance.UI.Root.AddChild(coins);
            coins.SetStyleAuto(null);
            coins.SetPosition(GameInstance.ScreenInfo.SetX(165), GameInstance.ScreenInfo.SetY(60));
            coins.SetSize(GameInstance.ScreenInfo.SetX(70), GameInstance.ScreenInfo.SetY(70));
            coins.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            coins.ImageRect = AssetsCoordinates.Generic.Icons.CoinsIcon;
            coins.Visible = false;

            //Wallet text
            Text wallet = new Text();
            GameInstance.UI.Root.AddChild(wallet);
            wallet.SetPosition(GameInstance.ScreenInfo.SetX(250), GameInstance.ScreenInfo.SetY(70));
            wallet.SetFont(_baseFont, GameInstance.ScreenInfo.SetX(30));
            int wallet_tot = CharacterManager.Instance.Wallet;
            wallet.Value = "" + wallet_tot;
            wallet.Visible = false;

            // SCREEN TITLE
            Button screen_title = new Button();
            GameInstance.UI.Root.AddChild(screen_title);
            screen_title.SetStyleAuto(null);
            screen_title.SetPosition(GameInstance.ScreenInfo.SetX(1500), GameInstance.ScreenInfo.SetY(50));
            screen_title.SetSize(GameInstance.ScreenInfo.SetX(400), GameInstance.ScreenInfo.SetY(100));
            screen_title.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            screen_title.Enabled = false;
            screen_title.ImageRect = AssetsCoordinates.Generic.Boxes.BoxTitle;

            Text buttonTitleText = new Text();
            screen_title.AddChild(buttonTitleText);
            buttonTitleText.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            buttonTitleText.SetPosition(0, 0);
            buttonTitleText.SetFont(_baseFont, GameInstance.ScreenInfo.SetX(30));
            buttonTitleText.Value = "SETTINGS";
        }


        void CreateLeftSection() {
         

            Sprite MiniGameScene = new Sprite();
            MiniGameScene.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            MiniGameScene.ImageRect = AssetsCoordinates.Generic.Boxes.MiniGameScreen;
            MiniGameScene.SetSize(GameInstance.ScreenInfo.SetX(940), GameInstance.ScreenInfo.SetY(530));
            MiniGameScene.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            MiniGameScene.SetPosition(GameInstance.ScreenInfo.SetX(50), GameInstance.ScreenInfo.SetX(250));
            GameInstance.UI.Root.AddChild(MiniGameScene);

            Sprite PausaButton = new Sprite();
            MiniGameScene.AddChild(PausaButton);
            PausaButton.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            PausaButton.ImageRect = AssetsCoordinates.Generic.Boxes.PausaButton;
            PausaButton.SetSize(GameInstance.ScreenInfo.SetX(50), GameInstance.ScreenInfo.SetY(50));
            PausaButton.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            PausaButton.SetPosition(GameInstance.ScreenInfo.SetX(40), GameInstance.ScreenInfo.SetY(40));

            Buttons = new Sprite();
            MiniGameScene.AddChild(Buttons);
            Buttons.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            Buttons.ImageRect = AssetsCoordinates.Generic.Boxes.ButtonsLeft;
            Buttons.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);

            AccBrakeBtn = new Sprite();
            MiniGameScene.AddChild(AccBrakeBtn);
            AccBrakeBtn.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            AccBrakeBtn.ImageRect = AssetsCoordinates.Generic.Boxes.AccBrakeRight;
            AccBrakeBtn.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);

            Sprite ButtonsA = new Sprite();
            ButtonsA.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            ButtonsA.ImageRect = AssetsCoordinates.Generic.Boxes.SelectionBoxGreenPart;
            ButtonsA.SetSize(GameInstance.ScreenInfo.SetX(345), GameInstance.ScreenInfo.SetY(81));
            ButtonsA.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            ButtonsA.SetPosition(GameInstance.ScreenInfo.SetX(50), GameInstance.ScreenInfo.SetY(800));
            //GameInstance.UI.Root.AddChild(ButtonsA);
            
            Text b1 = new Text();
            ButtonsA.AddChild(b1);
            b1.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            b1.SetPosition(0, 0);
            b1.SetFont(_baseFont, GameInstance.ScreenInfo.SetX(30));
            b1.Value = "BUTTONS";
                                            

            Button ButtonsB = new Button();
            ButtonsB.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            ButtonsB.ImageRect = AssetsCoordinates.Generic.Boxes.SelectionBoxWhitePart;
            ButtonsB.SetSize(GameInstance.ScreenInfo.SetX(595), GameInstance.ScreenInfo.SetY(81));
            ButtonsB.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            ButtonsB.SetPosition(GameInstance.ScreenInfo.SetX(395), GameInstance.ScreenInfo.SetY(800));
            //GameInstance.UI.Root.AddChild(ButtonsB);

            b2 = new Text();
            ButtonsB.AddChild(b2);
            b2.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            b2.SetPosition(0, 0);
            b2.SetFont(_baseFont, GameInstance.ScreenInfo.SetX(30));
            
                       
            ButtonsB.Pressed += args => {
                if (ButtonDimensions == 1) {
                    b2.Value = "MEDIUM";
                    ButtonDimensions = 2;
                    CharacterManager.Instance.ButtonDimension = 2;
                    SetButtonConfig();


                }
                else if (ButtonDimensions == 2) {
                    b2.Value = "LARGE";
                    ButtonDimensions = 3;
                    CharacterManager.Instance.ButtonDimension = 3;
                    SetButtonConfig();

                }
                else if (ButtonDimensions == 3) {
                    b2.Value = "SMALL";
                    ButtonDimensions = 1;
                    CharacterManager.Instance.ButtonDimension = 1;
                    SetButtonConfig();
                }

            };


            Sprite AccBrake = new Sprite();
            AccBrake.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            AccBrake.ImageRect = AssetsCoordinates.Generic.Boxes.SelectionBoxGreenPart;
            AccBrake.SetSize(GameInstance.ScreenInfo.SetX(345), GameInstance.ScreenInfo.SetY(81));
            AccBrake.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            AccBrake.SetPosition(GameInstance.ScreenInfo.SetX(50), GameInstance.ScreenInfo.SetY(900));
            //GameInstance.UI.Root.AddChild(AccBrake);

            Text a1 = new Text();
            AccBrake.AddChild(a1);
            a1.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            a1.SetPosition(0, 0);
            a1.SetFont(_baseFont, GameInstance.ScreenInfo.SetX(30));
            a1.Value = "ACC./BRAKE";

            Button AccBrakeB = new Button();
            AccBrakeB.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            AccBrakeB.ImageRect = AssetsCoordinates.Generic.Boxes.SelectionBoxWhitePart;
            AccBrakeB.SetSize(GameInstance.ScreenInfo.SetX(595), GameInstance.ScreenInfo.SetY(81));
            AccBrakeB.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            AccBrakeB.SetPosition(GameInstance.ScreenInfo.SetX(395), GameInstance.ScreenInfo.SetY(900));
            //GameInstance.UI.Root.AddChild(AccBrakeB);

            a2 = new Text();
            AccBrakeB.AddChild(a2);
            a2.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            a2.SetPosition(0, 0);
            a2.SetFont(_baseFont, GameInstance.ScreenInfo.SetX(30));
            

            AccBrakeB.Pressed += args => {
                 if(ButtonOrientation == 1) {
                    
                    ButtonOrientation = 2;
                    CharacterManager.Instance.ButtonOrientation = 2;
                    SetButtonConfig();
                 }
                 else if(ButtonOrientation == 2) {
                    a2.Value = "RIGHT";
                    ButtonOrientation = 1;
                     CharacterManager.Instance.ButtonOrientation = 1;
                    SetButtonConfig();

                 }

             };
        }


        void SetButtonConfig() {
            switch(ButtonOrientation) {
                case 1:
                    a2.Value = "RIGHT";
                    ButtonOrientation = 1;
                    CharacterManager.Instance.ButtonOrientation = 1;
                    Buttons.ImageRect = AssetsCoordinates.Generic.Boxes.ButtonsLeft;
                    AccBrakeBtn.ImageRect = AssetsCoordinates.Generic.Boxes.AccBrakeRight;

                    if(ButtonDimensions == 1) {
                        b2.Value = "SMALL";
                        Buttons.SetSize(GameInstance.ScreenInfo.SetX(120), GameInstance.ScreenInfo.SetY(224));
                        Buttons.SetPosition(GameInstance.ScreenInfo.SetX(30), GameInstance.ScreenInfo.SetY(270));
                        AccBrakeBtn.SetSize(GameInstance.ScreenInfo.SetX(120), GameInstance.ScreenInfo.SetY(120));
                        AccBrakeBtn.SetPosition(GameInstance.ScreenInfo.SetX(760), GameInstance.ScreenInfo.SetY(374));
                    }
                    else if(ButtonDimensions == 2) {
                        b2.Value = "MEDIUM";
                        Buttons.SetSize(GameInstance.ScreenInfo.SetX(130), GameInstance.ScreenInfo.SetY(243));
                        Buttons.SetPosition(GameInstance.ScreenInfo.SetX(30), GameInstance.ScreenInfo.SetY(250));
                        AccBrakeBtn.SetSize(GameInstance.ScreenInfo.SetX(130), GameInstance.ScreenInfo.SetY(130));
                        AccBrakeBtn.SetPosition(GameInstance.ScreenInfo.SetX(750), GameInstance.ScreenInfo.SetY(364));
                    }
                    else if(ButtonDimensions == 3) {
                        b2.Value = "LARGE";
                        Buttons.SetSize(GameInstance.ScreenInfo.SetX(140), GameInstance.ScreenInfo.SetY(262));
                        Buttons.SetPosition(GameInstance.ScreenInfo.SetX(30), GameInstance.ScreenInfo.SetY(230));
                        AccBrakeBtn.SetSize(GameInstance.ScreenInfo.SetX(140), GameInstance.ScreenInfo.SetY(140));
                        AccBrakeBtn.SetPosition(GameInstance.ScreenInfo.SetX(740), GameInstance.ScreenInfo.SetY(354));
                    }
                    break;
                case 2:
                    a2.Value = "LEFT";
                    ButtonOrientation = 2;
                    CharacterManager.Instance.ButtonOrientation = 2;
                    Buttons.ImageRect = AssetsCoordinates.Generic.Boxes.ButtonsRight;
                    //Buttons.SetPosition(GameInstance.ScreenInfo.SetX(730), GameInstance.ScreenInfo.SetY(230));
                    AccBrakeBtn.ImageRect = AssetsCoordinates.Generic.Boxes.AccBrakeLeft;
                    //AccBrakeBtn.SetPosition(GameInstance.ScreenInfo.SetX(50), GameInstance.ScreenInfo.SetY(364));

                    if(ButtonDimensions == 1) {
                        b2.Value = "SMALL";
                        Buttons.SetSize(GameInstance.ScreenInfo.SetX(120), GameInstance.ScreenInfo.SetY(224));
                        Buttons.SetPosition(GameInstance.ScreenInfo.SetX(730), GameInstance.ScreenInfo.SetY(270));
                        AccBrakeBtn.SetSize(GameInstance.ScreenInfo.SetX(120), GameInstance.ScreenInfo.SetY(120));
                        AccBrakeBtn.SetPosition(GameInstance.ScreenInfo.SetX(60), GameInstance.ScreenInfo.SetY(374));
                    }
                    else if(ButtonDimensions == 2) {
                        b2.Value = "MEDIUM";
                        Buttons.SetSize(GameInstance.ScreenInfo.SetX(130), GameInstance.ScreenInfo.SetY(243));
                        Buttons.SetPosition(GameInstance.ScreenInfo.SetX(730), GameInstance.ScreenInfo.SetY(250));
                        AccBrakeBtn.SetSize(GameInstance.ScreenInfo.SetX(130), GameInstance.ScreenInfo.SetY(130));
                        AccBrakeBtn.SetPosition(GameInstance.ScreenInfo.SetX(50), GameInstance.ScreenInfo.SetY(364));
                    }
                    else if(ButtonDimensions == 3) {
                        b2.Value = "LARGE";
                        Buttons.SetSize(GameInstance.ScreenInfo.SetX(140), GameInstance.ScreenInfo.SetY(262));
                        Buttons.SetPosition(GameInstance.ScreenInfo.SetX(730), GameInstance.ScreenInfo.SetY(230));
                        AccBrakeBtn.SetSize(GameInstance.ScreenInfo.SetX(140), GameInstance.ScreenInfo.SetY(140));
                        AccBrakeBtn.SetPosition(GameInstance.ScreenInfo.SetX(40), GameInstance.ScreenInfo.SetY(354));
                    }

                    break;
            }
        }


        void CreateRightSection() {

            var cont_base = GameInstance.ResourceCache.GetTexture2D("Textures/Garage/cont_base.png");
            Sprite containerR = new Sprite();
            containerR = GameInstance.UI.Root.CreateSprite();
            containerR.Texture = cont_base;
            containerR.SetSize(GameInstance.ScreenInfo.SetX(960), GameInstance.ScreenInfo.SetY(1080));
            containerR.SetPosition(GameInstance.ScreenInfo.SetX(960), GameInstance.ScreenInfo.SetY(0));
            containerR.ImageRect = new IntRect(0, 0, 56, 56);

            //green sprite
            Sprite SFX = new Sprite();
            SFX.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            SFX.ImageRect = AssetsCoordinates.Generic.Boxes.VolumeBarGreen;
            SFX.SetSize(GameInstance.ScreenInfo.SetX(170), GameInstance.ScreenInfo.SetY(100));
            SFX.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            SFX.SetPosition(GameInstance.ScreenInfo.SetX(50), GameInstance.ScreenInfo.SetY(225));
            containerR.AddChild(SFX);

            Text SFXtext = new Text();
            SFX.AddChild(SFXtext);
            SFXtext.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            SFXtext.SetPosition(GameInstance.ScreenInfo.SetX(-10), 0);
            SFXtext.SetFont(_baseFont, GameInstance.ScreenInfo.SetX(30));
            SFXtext.Value = "SFX";

            //slider
            Slider sfxSlider = new Slider() {
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(500), GameInstance.ScreenInfo.SetY(100)),
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(1190), GameInstance.ScreenInfo.SetY(225)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Range = 1,
                Value = SoundManager.Instance.EffectsGain,
                Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath),
                ImageRect = AssetsCoordinates.Generic.Boxes.VolumeBarWhite

            };
            GameInstance.UI.Root.AddChild(sfxSlider);

            var knob = sfxSlider.Knob;
            knob.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            knob.ImageRect = AssetsCoordinates.Generic.Boxes.VolumeBarKnob;
            knob.SetFixedSize(GameInstance.ScreenInfo.SetX(45), GameInstance.ScreenInfo.SetY(100));

            sfxSlider.SliderChanged += (SliderChangedEventArgs args) => {
                GameInstance.Audio.SetMasterGain(SoundType.Effect.ToString(), args.Value);
                SoundManager.Instance.EffectsGain = args.Value;
            };

            //"MAX" text
            Sprite MAX = new Sprite();
            MAX.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            MAX.ImageRect = AssetsCoordinates.Generic.Boxes.VolumeBarMAX;
            MAX.SetSize(GameInstance.ScreenInfo.SetX(130), GameInstance.ScreenInfo.SetY(100));
            MAX.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            MAX.SetPosition(GameInstance.ScreenInfo.SetX(750), GameInstance.ScreenInfo.SetY(225));
            containerR.AddChild(MAX);

            //green sprite
            Sprite Music = new Sprite();
            Music.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            Music.ImageRect = AssetsCoordinates.Generic.Boxes.VolumeBarGreen;
            Music.SetSize(GameInstance.ScreenInfo.SetX(170), GameInstance.ScreenInfo.SetY(100));
            Music.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            Music.SetPosition(GameInstance.ScreenInfo.SetX(50), GameInstance.ScreenInfo.SetY(350));
            containerR.AddChild(Music);

            Text MusicText = new Text();
            Music.AddChild(MusicText);
            MusicText.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            MusicText.SetPosition(GameInstance.ScreenInfo.SetX(-10), 0);
            MusicText.SetFont(_baseFont, GameInstance.ScreenInfo.SetX(30));
            MusicText.Value = "MUSIC";


            //slider
            Slider MusicSlider = new Slider() {
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(500), GameInstance.ScreenInfo.SetY(100)),
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(1190), GameInstance.ScreenInfo.SetY(350)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Range = 1,
                Value = SoundManager.Instance.MusicGain,
                Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath),
                ImageRect = AssetsCoordinates.Generic.Boxes.VolumeBarWhite
            };
            GameInstance.UI.Root.AddChild(MusicSlider);

            var knob2 = MusicSlider.Knob;
            knob2.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            knob2.ImageRect = AssetsCoordinates.Generic.Boxes.VolumeBarKnob;
            knob2.SetFixedSize(GameInstance.ScreenInfo.SetX(45), GameInstance.ScreenInfo.SetY(100));

            MusicSlider.SliderChanged += (SliderChangedEventArgs args) => {
                GameInstance.Audio.SetMasterGain(SoundType.Music.ToString(), args.Value);
                SoundManager.Instance.MusicGain = args.Value;
            };

            //"MAX" text
            Sprite MAX2 = new Sprite();
            MAX2.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            MAX2.ImageRect = AssetsCoordinates.Generic.Boxes.VolumeBarMAX;
            MAX2.SetSize(GameInstance.ScreenInfo.SetX(130), GameInstance.ScreenInfo.SetY(100));
            MAX2.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            MAX2.SetPosition(GameInstance.ScreenInfo.SetX(750), GameInstance.ScreenInfo.SetY(350));
            containerR.AddChild(MAX2);

            // FB
            Button Facebook = new Button();
            containerR.AddChild(Facebook);
            Facebook.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            Facebook.ImageRect = AssetsCoordinates.Generic.Boxes.Facebook;
            Facebook.SetSize(GameInstance.ScreenInfo.SetX(150), GameInstance.ScreenInfo.SetY(150));
            Facebook.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            Facebook.SetPosition(GameInstance.ScreenInfo.SetX(300), GameInstance.ScreenInfo.SetY(500));
            Facebook.Pressed += args => {
                OpenBrowser("https://www.facebook.com/smartroadsense/");
            };

            // TWITTER
            Button Twitter = new Button();
            containerR.AddChild(Twitter);
            Twitter.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            Twitter.ImageRect = AssetsCoordinates.Generic.Boxes.Tweeter;
            Twitter.SetSize(GameInstance.ScreenInfo.SetX(150), GameInstance.ScreenInfo.SetY(150));
            Twitter.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            Twitter.SetPosition(GameInstance.ScreenInfo.SetX(500), GameInstance.ScreenInfo.SetY(500));
            Twitter.Pressed += args => {
                OpenBrowser("https://twitter.com/smartroadsense");
            };

            /*
            Button Credits = new Button();
            containerR.AddChild(Credits);
            Credits.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            Credits.ImageRect = AssetsCoordinates.Generic.Boxes.Credits;
            Credits.SetSize(GameInstance.ScreenInfo.SetX(600), GameInstance.ScreenInfo.SetY(100));
            Credits.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
            Credits.SetPosition(GameInstance.ScreenInfo.SetX(200), GameInstance.ScreenInfo.SetY(800));

            Text CreditsText = new Text();
            Credits.AddChild(CreditsText);
            CreditsText.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            CreditsText.SetPosition(0, 0);
            CreditsText.SetFont(_baseFont, GameInstance.ScreenInfo.SetX(30));
            CreditsText.Value = "CREDITS";
            */           
        }

        void OpenBrowser(string url) {
#if __ANDROID__
            var uri = global::Android.Net.Uri.Parse(url);
            var intent = new Intent(Intent.ActionView, uri);
            CrossCurrentActivity.Current.Activity.StartActivity(intent);

#elif __IOS__
            UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
#endif
        }
    }
}
