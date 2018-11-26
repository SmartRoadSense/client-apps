using Xamarin.Forms;

namespace SmartRoadSense
{
    public static class StyleSettings
    {
        static readonly Color ThemePrimary = Color.FromRgb(42, 105, 255);
        static readonly Color ThemeSecondary = Color.FromRgb(255, 65, 24);
        static readonly Color ThemeTerziary = Color.FromRgb(132, 204, 13);
        static readonly Color ThemePrimaryDark = Color.FromRgb(2, 9, 25);
        static readonly Color ThemePrimaryDarkLightened = Color.FromRgb(34, 41, 57);
        static readonly Color ThemePrimaryPressed = Color.FromRgb(24, 63, 153);
        static readonly Color DrawerBackground = Color.FromRgb(17, 17, 17);
        static readonly Color DefaultBackground = Color.FromRgb(51, 51, 51);
        static readonly Color TextOnDark = Color.FromRgb(255, 255, 255);
        static readonly Color SubtleTextOnDark = Color.FromRgb(136, 136, 136);
        static readonly Color SubduedTextOnDark = Color.FromRgb(112, 121, 140);
        static readonly Color TextOnBright = Color.FromRgb(51, 51, 51);
        static readonly Color SubtleTextOnBright = Color.FromRgb(187, 187, 187);
        static readonly Color LightGray = Color.FromRgb(221, 221, 221);
        static readonly Color LightGrayPressed = Color.FromRgb(204, 204, 204);
        static readonly Color TextOnLightGray = Color.FromRgb(0, 0, 0);
        static readonly Color QualityGood = Color.FromRgb(173, 210, 172);
        static readonly Color QualityBad = Color.FromRgb(209, 152, 171);
        static readonly Color Error = Color.FromRgb(229, 28, 35);

        public static Color ThemePrimaryColor() { return ThemePrimary; }
        public static Color ThemeSecondaryColor() { return ThemeSecondary; }
        public static Color ThemeTerziaryColor() { return ThemeTerziary; }
        public static Color ThemePrimaryDarkColor() { return ThemePrimaryDark; }
        public static Color ThemePrimaryDarkLightenedColor() { return ThemePrimaryDarkLightened; }
        public static Color ThemePrimaryPressedColor() { return ThemePrimaryPressed; }
        public static Color DrawerBackgroundColor() { return DrawerBackground; }
        public static Color DefaultBackgroundColor() { return DefaultBackground; }
        public static Color TextOnDarkColor() { return TextOnDark; }
        public static Color SubtleTextOnDarkColor() { return SubtleTextOnDark; }
        public static Color SubduedTextOnDarkColor() { return SubduedTextOnDark; }
        public static Color TextOnBrightColor() { return TextOnBright; }
        public static Color SubtleTextOnBrightColor() { return SubtleTextOnBright; }
        public static Color LightGrayColor() { return LightGray; }
        public static Color LightGrayPressedColor() { return LightGrayPressed; }
        public static Color TextOnLightGrayColor() { return TextOnLightGray; }
        public static Color QualityGoodColor() { return QualityGood; }
        public static Color QualityBadColor() { return QualityBad; }
        public static Color ErrorColor() { return Error; }

        public static Color InterpolateTextColor(Color a, Color b, float linearInterpolation)
        {
            Color finalColor = new Color(
                (a.R + (linearInterpolation * (b.R - a.R))),
                (a.G + (linearInterpolation * (b.G - a.G))),
                (a.B + (linearInterpolation * (b.B - a.B))),
                (a.A + (linearInterpolation * (b.A - a.A)))
            );
            return finalColor;
        }
    }

    public static class ExtensionMethods
    {
        public static string GetHexString(this Color color)
        {
            var red = (int)(color.R * 255);
            var green = (int)(color.G * 255);
            var blue = (int)(color.B * 255);
            var alpha = (int)(color.A * 255);
            var hex = $"#{alpha:X2}{red:X2}{green:X2}{blue:X2}";

            return hex;
        }
    }
}
