using System;
using UIKit;
using SmartRoadSense.Shared;
using CoreGraphics;
using CoreImage;

namespace SmartRoadSense.iOS
{
	public static class StyleSettings
	{
		private static UIColor ThemePrimary = UIColor.FromRGB (42, 105, 255);
		private static UIColor ThemeSecondary = UIColor.FromRGB (255, 65, 24);
		private static UIColor ThemeTerziary = UIColor.FromRGB (132, 204, 13);
		private static UIColor ThemePrimaryDark = UIColor.FromRGB (2, 9, 25);
		private static UIColor ThemePrimaryDarkLightened = UIColor.FromRGB (34, 41, 57);
		private static UIColor ThemePrimaryPressed = UIColor.FromRGB (24, 63, 153);
		private static UIColor DrawerBackground = UIColor.FromRGB (17, 17, 17);
		private static UIColor DefaultBackground = UIColor.FromRGB (51, 51, 51);
		private static UIColor TextOnDark = UIColor.FromRGB (255, 255, 255);
		private static UIColor SubtleTextOnDark = UIColor.FromRGB (136, 136, 136);
		private static UIColor SubduedTextOnDark = UIColor.FromRGB (112, 121, 140);
		private static UIColor TextOnBright = UIColor.FromRGB(51, 51, 51);
		private static UIColor SubtleTextOnBright = UIColor.FromRGB (187, 187, 187);
		private static UIColor LightGray = UIColor.FromRGB (221, 221, 221);
		private static UIColor LightGrayPressed = UIColor.FromRGB (204, 204, 204);
		private static UIColor TextOnLightGray = UIColor.FromRGB (0, 0, 0);
		private static UIColor QualityGood = UIColor.FromRGB(173, 210, 172);
		private static UIColor QualityBad = UIColor.FromRGB(209, 152, 171);
		private static UIColor Error = UIColor.FromRGB (229, 28, 35);

		public static UIColor ThemePrimaryColor() { return ThemePrimary; }
		public static UIColor ThemeSecondaryColor() { return ThemeSecondary; }
		public static UIColor ThemeTerziaryColor() { return ThemeTerziary; }
		public static UIColor ThemePrimaryDarkColor() { return ThemePrimaryDark; }
		public static UIColor ThemePrimaryDarkLightenedColor() { return ThemePrimaryDarkLightened; }
		public static UIColor ThemePrimaryPressedColor() { return ThemePrimaryPressed; }
		public static UIColor DrawerBackgroundColor() { return DrawerBackground; }
		public static UIColor DefaultBackgroundColor() { return DefaultBackground; }
		public static UIColor TextOnDarkColor() { return TextOnDark; }
		public static UIColor SubtleTextOnDarkColor() { return SubtleTextOnDark; }
		public static UIColor SubduedTextOnDarkColor() { return SubduedTextOnDark; }
		public static UIColor TextOnBrightColor() { return TextOnBright; }
		public static UIColor SubtleTextOnBrightColor() { return SubtleTextOnBright; }
		public static UIColor LightGrayColor() { return LightGray; }
		public static UIColor LightGrayPressedColor() { return LightGrayPressed; }
		public static UIColor TextOnLightGrayColor() { return TextOnLightGray; }
		public static UIColor QualityGoodColor() { return QualityGood; }
		public static UIColor QualityBadColor() { return QualityBad; }
		public static UIColor ErrorColor() { return Error; }

		public static UIColor InterpolateTextColor(UIColor a, UIColor b, float linearInterpolation) {
			System.nfloat[] colorsA = new System.nfloat[4];
			System.nfloat[] colorsB = new System.nfloat[4];
			a.GetRGBA (out colorsA [0], out colorsA [1], out colorsA [2], out colorsA [3]);
			b.GetRGBA (out colorsB [0], out colorsB [1], out colorsB [2], out colorsB [3]);


			UIColor finalColor = new UIColor(
				(colorsA[0] + (linearInterpolation * (colorsB[0] - colorsA[0]))),
				(colorsA[1] + (linearInterpolation * (colorsB[1] - colorsA[1]))),
				(colorsA[2] + (linearInterpolation * (colorsB[2] - colorsA[2]))),
				(colorsA[3] + (linearInterpolation * (colorsB[3] - colorsA[3])))
			);
			return finalColor;
		}
	}
}

