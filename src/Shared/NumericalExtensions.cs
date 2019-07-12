using System;
using System.Globalization;

namespace SmartRoadSense.Shared {

    public static class NumericalExtensions {

        public static string ToUnits(this int i) {
            double digits = Math.Floor(Math.Log10(i));
            double multiplicator = Math.Pow(10, digits);
            double r = multiplicator * Math.Floor(i / multiplicator);
            return string.Format(CultureInfo.InvariantCulture, "{0}s", r);
        }

        public static string ToUnits(this double d) {
            double digits = Math.Floor(Math.Log10(d));
            double multiplicator = Math.Pow(10, digits);
            double r = multiplicator * Math.Floor(d / multiplicator);
            return string.Format(CultureInfo.InvariantCulture, "{0}s", r);
        }

    }

}
