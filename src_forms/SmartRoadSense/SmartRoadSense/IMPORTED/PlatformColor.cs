using System;
using Xamarin.Forms;

namespace SmartRoadSense {

    /// <summary>
    /// Platform-abstracted color representation.
    /// </summary>
    public struct PlatformColor
    {
        public readonly Color Color;

        public PlatformColor(byte r, byte g, byte b)
        {
            Color = Color.FromRgba(r, g, b, 255);
        }
        /*
#if __ANDROID__
        public PlatformColor(byte r, byte g, byte b) {
            Color = global::Android.Graphics.Color.Argb(255, r, g, b);
        }

        public readonly global::Android.Graphics.Color Color;
#elif __IOS__
        public PlatformColor(byte r, byte g, byte b) {
            Color = new global::UIKit.UIColor(
                (float)(r / 255.0),
                (float)(g / 255.0),
                (float)(b / 255.0),
                1f
            );
        }

        public readonly global::UIKit.UIColor Color;
        }
#endif
*/
    }
}
