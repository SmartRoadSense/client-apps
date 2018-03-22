using System;

namespace SmartRoadSense.Shared {

    /// <summary>
    /// Platform-abstracted color representation.
    /// </summary>
    public struct PlatformColor {

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
#elif WINDOWS_PHONE_APP || DESKTOP
        public PlatformColor(byte r, byte g, byte b) {
            //TODO
        }
#else
#error Unrecognized platform
#endif

    }

}
