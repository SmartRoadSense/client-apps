using Urho;
using Urho.Gui;
using Urho.Resources;

namespace SmartRoadSense.Shared {
    public class SceneCredits : BaseScene {

        Font font;
        ResourceCache cache;
        public SceneCredits(Game game) : base(game) {

            cache = GameInstance.ResourceCache;
            font = cache.GetFont(GameInstance.defaultFont);
            CreateBackground();
            CreateTopBar();
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
            GameInstance.UI.Root.AddChild(coins);
            coins.SetStyleAuto(null);
            coins.SetPosition(GameInstance.ScreenInfo.SetX(180), GameInstance.ScreenInfo.SetY(60));
            coins.SetSize(GameInstance.ScreenInfo.SetX(75), GameInstance.ScreenInfo.SetY(70));
            coins.Texture = GameInstance.ResourceCache.GetTexture2D(AssetsCoordinates.Generic.Icons.ResourcePath);
            coins.ImageRect = AssetsCoordinates.Generic.Icons.IconCoin;


            //Wallet text
            Text wallet = new Text();
            coins.AddChild(wallet);
            wallet.SetPosition(GameInstance.ScreenInfo.SetX(90), GameInstance.ScreenInfo.SetY(10));
            wallet.SetFont(font, GameInstance.ScreenInfo.SetX(30));
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

            Text buttonTitleText = new Text();
            screen_title.AddChild(buttonTitleText);
            buttonTitleText.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
            buttonTitleText.SetPosition(0, 0);
            buttonTitleText.SetFont(font, GameInstance.ScreenInfo.SetX(30));
            buttonTitleText.Value = "CREDITS";
        }
    }
}
