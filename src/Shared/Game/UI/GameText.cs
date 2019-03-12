using Urho;
using Urho.Gui;

namespace SmartRoadSense.Shared {
    public static class GameText {

        public static Text CreateText(UIElement parent, ScreenInfoRatio screenInfo, Font font, int fontSize, int xPos, int yPos, HorizontalAlignment hAlign, VerticalAlignment vAlign, string value) {
            var text = CreateText(parent, screenInfo, font, fontSize, xPos, yPos, hAlign, vAlign);
            text.Value = value;

            return text;
        }

        public static Text CreateText(UIElement parent, ScreenInfoRatio screenInfo, Font font, int fontSize, int xPos, int yPos, HorizontalAlignment hAlign, VerticalAlignment vAlign) {
            var text = new Text();
            parent.AddChild(text);

            text.SetAlignment(hAlign, vAlign);
            text.SetPosition(screenInfo.SetX(xPos), screenInfo.SetY(yPos));
            text.SetFont(font, screenInfo.SetX(fontSize));
            text.SetColor(Color.Black);

            return text;
        }
    }
}
