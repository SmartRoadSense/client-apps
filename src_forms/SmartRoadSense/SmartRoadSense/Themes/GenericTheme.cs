using System;
using Xamarin.Forms;

namespace SmartRoadSense
{
    public abstract class GenericTheme
    {
        abstract public string ThemePrimaryColorHexString { get; }
        abstract public string ThemeSecondaryColorHexString { get; }
        abstract public string ThemeTerziaryColorHexString { get; }
        abstract public string ThemePrimaryDarkColorHexString { get; }
        abstract public string ThemePrimaryDarkLightenedColorHexString { get; }
        abstract public string ThemePrimaryPressedColorHexString { get; }
        abstract public string DrawerBackgroundColorHexString { get; }
        abstract public string DefaultBackgroundColorHexString { get; }
        abstract public string TextOnDarkColorHexString { get; }
        abstract public string SubtleTextOnDarkColorHexString { get; }
        abstract public string SubduedTextOnDarkColorHexString { get; }
        abstract public string TextOnBrightColorHexString { get; }
        abstract public string SubtleTextOnBrightColorHexString { get; }
        abstract public string LightGrayColorHexString { get; }
        abstract public string LightGrayPressedColorHexString { get; }
        abstract public string TextOnLightGrayColorHexString { get; }
        abstract public string QualityGoodColorHexString { get; }
        abstract public string QualityBadColorHexString { get; }
        abstract public string ErrorColorHexString { get; }
    }
}
