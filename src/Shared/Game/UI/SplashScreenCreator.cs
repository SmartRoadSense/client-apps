using Urho;
using Urho.Gui;
using Urho.Urho2D;
using System;
using Urho.Resources;

namespace SmartRoadSense.Shared {
    public static class SplashScreenCreator {

        /// <summary>
        /// Returns the name of the UI Element
        /// </summary>
        /// <returns>Splash screen.</returns>
        /// <param name="GameInstance">Game instance.</param>
        public static string CreateSplashScreen(Game GameInstance, Node parent, bool randomLevel = false) {
            // Get data
            var data = TrackManager.Instance.LoadingScreenFacts;
            var rnd = new Random();
            var argIdx = rnd.Next(0, data.LoadingScreens.Count);
            var arg = data.LoadingScreens[argIdx];
            var factIdx = rnd.Next(0, arg.Facts.Count);
            var fact = arg.Facts[factIdx];

            // get poster asset
            IntRect poster;
            switch(arg.Type) {
                case LoadingScreenType.CLI:
                    poster = AssetsCoordinates.Backgrounds.LoadingScreen.Posters.Climate;
                    break;
                case LoadingScreenType.ENV:
                    poster = AssetsCoordinates.Backgrounds.LoadingScreen.Posters.Environment;
                    break;
                case LoadingScreenType.RES:
                    poster = AssetsCoordinates.Backgrounds.LoadingScreen.Posters.Resources;
                    break;
                case LoadingScreenType.WASTE:
                    poster = AssetsCoordinates.Backgrounds.LoadingScreen.Posters.Waste;
                    break;
                case LoadingScreenType.WILD:
                    poster = AssetsCoordinates.Backgrounds.LoadingScreen.Posters.Wild;
                    break;
                default:
                    poster = AssetsCoordinates.Backgrounds.LoadingScreen.Posters.Environment;
                    break;
            }

            var splashscreen = new Window {
                Name = "SplashUI",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(0)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(1920), GameInstance.ScreenInfo.SetY(1080)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Backgrounds.FixedBackground.ResourcePath),
                ImageRect = AssetsCoordinates.Backgrounds.FixedBackground.ImageRect,
                Priority = 999
            };
            GameInstance.UI.Root.AddChild(splashscreen);

            // TOP BAR
            var topBar = new Sprite();
            splashscreen.AddChild(topBar);
            topBar.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.TopBar.ResourcePath);
            topBar.ImageRect = AssetsCoordinates.Generic.TopBar.Rectangle;
            topBar.Opacity = 0.5f;
            topBar.SetPosition(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(30));
            topBar.SetSize(GameInstance.ScreenInfo.SetX(2000), GameInstance.ScreenInfo.SetY(120));

            Button screenTitle = new Button();
            //screenTitle.SetStyleAuto(null);
            screenTitle.HorizontalAlignment = HorizontalAlignment.Right;
            screenTitle.VerticalAlignment = VerticalAlignment.Center;
            screenTitle.SetPosition(GameInstance.ScreenInfo.SetX(-150), GameInstance.ScreenInfo.SetY(0));
            screenTitle.SetSize(GameInstance.ScreenInfo.SetX(400), GameInstance.ScreenInfo.SetY(95));
            screenTitle.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath);
            screenTitle.ImageRect = AssetsCoordinates.Generic.Boxes.BoxTitle;
            screenTitle.Enabled = false;
            screenTitle.UseDerivedOpacity = false;
            topBar.AddChild(screenTitle);

            Text buttonTitleText = new Text();
            screenTitle.AddChild(buttonTitleText);
            buttonTitleText.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Center);
            buttonTitleText.SetPosition(GameInstance.ScreenInfo.SetX(20), GameInstance.ScreenInfo.SetY(0));
            buttonTitleText.SetFont(GameInstance.ResourceCache.GetFont(GameInstance.defaultFont), GameInstance.ScreenInfo.SetX(35));
            buttonTitleText.Value = "LOADING";
            buttonTitleText.UseDerivedOpacity = false;

            BorderImage wheel = new BorderImage();
            screenTitle.AddChild(wheel);
            wheel.SetAlignment(HorizontalAlignment.Right, VerticalAlignment.Center);
            wheel.SetPosition(GameInstance.ScreenInfo.SetX(-15), GameInstance.ScreenInfo.SetY(0));
            wheel.SetSize(100, 100);
            wheel.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            wheel.ImageRect = AssetsCoordinates.Generic.Icons.LoadingWheel;
            wheel.UseDerivedOpacity = false;
            /*
            ObjectAnimation wheelAnimation = new ObjectAnimation();
            ValueAnimation positionAnimation = new ValueAnimation();
            positionAnimation.InterpolationMethod = InterpMethod.Spline;
            positionAnimation.SetKeyFrame(0.0f, new Vector2(1.0f,1.0f));
            positionAnimation.SetKeyFrame(1.0f, new Vector2(0.0f, 0.0f));
            positionAnimation.SetKeyFrame(2.0f, new Vector2(-1.0f, -1.0f));
            positionAnimation.SetKeyFrame(3.0f, new Vector2(0.0f, 1.0f));
            positionAnimation.SetKeyFrame(4.0f, new Vector2(1.0f, 1.0f));


            wheelAnimation.AddAttributeAnimation("Position", positionAnimation);
            wheel.ObjectAnimation = wheelAnimation;
            wheel.AnimationEnabled = true;
            wheel.ApplyAttributes();
            */
            // BODY
            if(randomLevel) {
                var text = new Text();
                text.SetFont(GameInstance.ResourceCache.GetFont(GameInstance.defaultFont), 17);
                text.SetPosition(GameInstance.ScreenInfo.SetX(20), GameInstance.ScreenInfo.SetY(172));
                text.Value = "*Coins and items collected in the random level are not saved.";
                text.UseDerivedOpacity = false;
                splashscreen.AddChild(text);
            }

            var posterImage = new Window {
                Name = "PosterUI",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(200), GameInstance.ScreenInfo.SetY(50)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(500), GameInstance.ScreenInfo.SetY(636)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Backgrounds.LoadingScreen.Posters.Path),
                ImageRect = poster
            };
            splashscreen.AddChild(posterImage);

            var textWindow = new Window {
                Name = "splashScreenTextWindow",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(750), GameInstance.ScreenInfo.SetY(50)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(1000), GameInstance.ScreenInfo.SetY(636)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };
            textWindow.SetColor(Color.Transparent);
            splashscreen.AddChild(textWindow);

            var titleBox = new Text {
                Name = "splashScreenTitle",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(10), GameInstance.ScreenInfo.SetY(-20)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(1000), GameInstance.ScreenInfo.SetY(60)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Wordwrap = true
            };

            titleBox.SetFont(GameInstance.ResourceCache.GetFont(GameInstance.defaultFont), GameInstance.ScreenInfo.SetX(50));
            titleBox.Value = arg.Title;
            textWindow.AddChild(titleBox);

            var textBox = new Text {
                Name = "splashScreenText",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(10), GameInstance.ScreenInfo.SetY(75)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(1000), GameInstance.ScreenInfo.SetY(350)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Wordwrap = true
            };

            textBox.SetFont(GameInstance.ResourceCache.GetFont(GameInstance.defaultFont), GameInstance.ScreenInfo.SetX(30));
            textBox.Value = fact.Message;
            textWindow.AddChild(textBox);

            var btnContinue = new Button {
                Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Boxes.ResourcePath),
                ImageRect = AssetsCoordinates.Generic.Boxes.GroupSelected,
                Name = "ButtonContinue",
                Position = new IntVector2(GameInstance.ScreenInfo.SetX(0), GameInstance.ScreenInfo.SetY(0)),
                Size = new IntVector2(GameInstance.ScreenInfo.SetX(400), GameInstance.ScreenInfo.SetY(95)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            btnContinue.Visible = false;
            textWindow.AddChild(btnContinue);

            Text btnContinueText = new Text();
            btnContinueText.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            btnContinueText.SetPosition(GameInstance.ScreenInfo.SetX(-10), GameInstance.ScreenInfo.SetY(0));
            btnContinueText.SetFont(GameInstance.ResourceCache.GetFont(GameInstance.defaultFont), GameInstance.ScreenInfo.SetX(35));
            btnContinueText.Value = "CONTINUE";
            btnContinue.AddChild(btnContinueText);

            return splashscreen.Name;
        }
    }
}
