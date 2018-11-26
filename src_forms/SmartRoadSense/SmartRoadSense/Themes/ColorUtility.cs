using System;
using Xamarin.Forms;

namespace SmartRoadSense
{
    public class ColorUtility
    {
        public string ThemePrimaryColor;
        public string ThemeSecondaryColor;

        public ColorUtility()
        {
            // TODO: switch theme here
            DefineTheme(new BaseTheme());
        }

        void DefineTheme(GenericTheme ThemeData)
        {
            ThemePrimaryColor = ThemeData.ThemePrimaryColorHexString;
            ThemeSecondaryColor = ThemeData.ThemeSecondaryColorHexString;
        }
    }
}