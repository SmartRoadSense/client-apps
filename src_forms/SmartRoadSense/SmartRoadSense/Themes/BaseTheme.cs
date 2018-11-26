using System;
using Xamarin.Forms;

namespace SmartRoadSense
{
    public class BaseTheme : GenericTheme
    {
        public override string ThemePrimaryColorHexString { get => StyleSettings.ThemePrimaryColor().GetHexString(); }
        public override string ThemeSecondaryColorHexString { get => StyleSettings.ThemeSecondaryColor().GetHexString(); }
        public override string ThemeTerziaryColorHexString { get => StyleSettings.ThemeTerziaryColor().GetHexString(); }
        public override string ThemePrimaryDarkColorHexString { get => StyleSettings.ThemePrimaryDarkColor().GetHexString(); }
        public override string ThemePrimaryDarkLightenedColorHexString { get => StyleSettings.ThemePrimaryDarkLightenedColor().GetHexString(); }
        public override string ThemePrimaryPressedColorHexString { get => StyleSettings.ThemePrimaryPressedColor().GetHexString(); }
        public override string DrawerBackgroundColorHexString { get => StyleSettings.DrawerBackgroundColor().GetHexString(); }
        public override string DefaultBackgroundColorHexString { get => StyleSettings.DefaultBackgroundColor().GetHexString(); }
        public override string TextOnDarkColorHexString { get => StyleSettings.TextOnDarkColor().GetHexString(); }
        public override string SubtleTextOnDarkColorHexString { get => StyleSettings.SubtleTextOnDarkColor().GetHexString(); }
        public override string SubduedTextOnDarkColorHexString { get => StyleSettings.SubduedTextOnDarkColor().GetHexString(); }
        public override string TextOnBrightColorHexString { get => StyleSettings.TextOnBrightColor().GetHexString(); }
        public override string SubtleTextOnBrightColorHexString { get => StyleSettings.SubtleTextOnBrightColor().GetHexString(); }
        public override string LightGrayColorHexString { get => StyleSettings.LightGrayColor().GetHexString(); }
        public override string LightGrayPressedColorHexString { get => StyleSettings.LightGrayPressedColor().GetHexString(); }
        public override string TextOnLightGrayColorHexString { get => StyleSettings.TextOnLightGrayColor().GetHexString(); }
        public override string QualityGoodColorHexString { get => StyleSettings.QualityGoodColor().GetHexString(); }
        public override string QualityBadColorHexString { get => StyleSettings.QualityBadColor().GetHexString(); }
        public override string ErrorColorHexString { get => StyleSettings.ErrorColor().GetHexString(); }
    }
}
