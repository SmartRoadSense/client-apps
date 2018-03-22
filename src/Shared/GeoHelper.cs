using System;

namespace SmartRoadSense.Shared {

    public class GeoHelper {
        public const double MeanEarthRadius = 6378.16;

        /// <summary>
        /// Converts a degree measurement to radians.
        /// </summary>
        public static double FromDegreesToRadians(double d) {
            return d * Math.PI / 180.0;
        }

        /// <summary>
        /// Computes the approximate distance in kilometers between two geographical points.
        /// Uses a "Haversine" great circle calculation, on an ideal sphere. Not exact close to equator and poles.
        /// </summary>
        /// <returns>Distance in kilometers.</returns>
        /// <param name="lat1">Latitude of first point.</param>
        /// <param name="lng1">Longitude of first point.</param>
        /// <param name="lat2">Latitude of second point.</param>
        /// <param name="lng2">Longitude of second point.</param>
        public static double DistanceBetweenPoints(double lat1, double lng1, double lat2, double lng2) {
            double dLng = FromDegreesToRadians(lng2 - lng1);
            double dLat = FromDegreesToRadians(lat2 - lat1);

            double radLat1 = FromDegreesToRadians(lat1);
            double radLat2 = FromDegreesToRadians(lat2);

            double a = (Math.Sin(dLat / 2) * Math.Sin(dLat / 2)) + Math.Cos(radLat1) * Math.Cos(radLat2) * (Math.Sin(dLng / 2) * Math.Sin(dLng / 2));
            double angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return angle * MeanEarthRadius;
        }

    }

}
