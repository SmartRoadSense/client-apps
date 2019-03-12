using System;
using Urho;
using Urho.Gui;

namespace SmartRoadSense.Shared {
    public static class GameButton {

        public static Button CreateButton (UIElement parent, ScreenInfoRatio screenInfo, int posX, int posY, int width, int height, HorizontalAlignment hAlign, VerticalAlignment vAlign) {
            Button button = new Button();
            parent.AddChild(button);

            button.SetStyleAuto(null);
            button.SetPosition(screenInfo.SetX(posX), screenInfo.SetY(posY));
            button.SetAlignment(hAlign, vAlign);
            button.SetSize(screenInfo.SetX(width), screenInfo.SetY(height));
            button.SetColor(Color.White);

            return button;
        }
    }
}
