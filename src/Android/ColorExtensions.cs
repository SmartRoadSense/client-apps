using System;

using Android.Views;
using Android.Graphics;
using Android.Widget;

namespace SmartRoadSense.Android {

    public static class ColorExtensions {

        public static void InterpolateTextColor(this TextView v, Color a, Color b, float linearInterpolation) {
            var finalColor = new Color(
                (int)(a.R + (int)(linearInterpolation * (b.R - a.R))),
                (int)(a.G + (int)(linearInterpolation * (b.G - a.G))),
                (int)(a.B + (int)(linearInterpolation * (b.B - a.B))),
                (int)(a.A + (int)(linearInterpolation * (b.A - a.A)))
            );

            v.SetTextColor(finalColor);
        }

    }

}

