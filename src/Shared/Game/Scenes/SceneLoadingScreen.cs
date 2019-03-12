using Urho.Gui;

namespace SmartRoadSense.Shared
{
    public class SceneLoadingScreen : BaseScene
    {
        readonly Font _font;

        public SceneLoadingScreen(Game game) : base(game)
        {
            _font = GameInstance.ResourceCache.GetFont(GameInstance.defaultFont);

            CreateBackground();
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
    }
}
