using System;
using UIKit;
using CoreGraphics;
using System.Drawing;

namespace SmartRoadSense.iOS
{
	public static class ChangeImageColor
	{
		public static UIImage GetColoredImage(string imageName, UIColor color)
		{
			UIImage image = UIImage.FromBundle(imageName);
			UIImage coloredImage = null;

			UIGraphics.BeginImageContext(image.Size);
			using (CGContext context = UIGraphics.GetCurrentContext())
			{

				context.TranslateCTM(0, image.Size.Height);
				context.ScaleCTM(1.0f, -1.0f);

				var rect = new RectangleF(0, 0, (float)image.Size.Width, (float)image.Size.Height);

				// draw image, (to get transparancy mask)
				context.SetBlendMode(CGBlendMode.Normal);
				context.DrawImage(rect, image.CGImage);

				// draw the color using the sourcein blend mode so its only draw on the non-transparent pixels
				context.SetBlendMode(CGBlendMode.SourceIn);
				context.SetFillColor(color.CGColor);
				context.FillRect(rect);

				coloredImage = UIGraphics.GetImageFromCurrentImageContext();
				UIGraphics.EndImageContext();
			}
			return coloredImage;
		}
	}
}

