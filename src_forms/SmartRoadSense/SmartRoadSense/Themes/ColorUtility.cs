using System;
using Xamarin.Forms;

namespace SmartRoadSense
{
    public class ColorUtility
    {
        public string ThemePrimaryColor;
        public string ThemeSecondaryColor;
        public string ThemeTerziaryColor;
        public string ThemePrimaryDarkColor;
        public string ThemePrimaryDarkLightenedColor;
        public string ThemePrimaryPressedColor;
        public string DrawerBackgroundColor;
        public string DefaultBackgroundColor;
        public string TextOnDarkColor;
        public string SubtleTextOnDarkColor;
        public string SubduedTextOnDarkColor;
        public string TextOnBrightColor;
        public string SubtleTextOnBrightColor;
        public string LightGrayColor;
        public string LightGrayPressedColor;
        public string TextOnLightGrayColor;
        public string QualityGoodColor;
        public string QualityBadColor;
        public string ErrorColor;

        public ColorUtility()
        {
            // TODO: switch theme here
            DefineTheme(new BaseTheme());
        }

        void DefineTheme(GenericTheme ThemeData)
        {
            ThemePrimaryColor = ThemeData.ThemePrimaryColorHexString;
            ThemeSecondaryColor = ThemeData.ThemeSecondaryColorHexString;
            ThemeTerziaryColor = ThemeData.ThemeTerziaryColorHexString;
            ThemePrimaryDarkColor = ThemeData.ThemePrimaryDarkColorHexString;
            ThemePrimaryDarkLightenedColor = ThemeData.ThemePrimaryDarkLightenedColorHexString;
            ThemePrimaryPressedColor = ThemeData.LightGrayPressedColorHexString;
            DrawerBackgroundColor = ThemeData.DrawerBackgroundColorHexString;
            DefaultBackgroundColor = ThemeData.DefaultBackgroundColorHexString;
            TextOnDarkColor = ThemeData.TextOnDarkColorHexString;
            SubtleTextOnDarkColor = ThemeData.SubtleTextOnDarkColorHexString;
            SubduedTextOnDarkColor = ThemeData.SubduedTextOnDarkColorHexString;
            TextOnBrightColor = ThemeData.TextOnBrightColorHexString;
            SubtleTextOnBrightColor = ThemeData.SubtleTextOnBrightColorHexString;
            LightGrayColor = ThemeData.LightGrayColorHexString;
            LightGrayPressedColor = ThemeData.LightGrayPressedColorHexString;
            TextOnLightGrayColor = ThemeData.TextOnLightGrayColorHexString;
            QualityGoodColor = ThemeData.QualityGoodColorHexString;
            QualityBadColor = ThemeData.QualityBadColorHexString;
            ErrorColor = ThemeData.ErrorColorHexString;
        }
    }
}