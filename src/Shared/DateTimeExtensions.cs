using System;

namespace SmartRoadSense.Shared {

    public static class DateTimeExtensions {

        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts a date time value to milliseconds from the beginning of the Unix epoch.
        /// </summary>
        public static long ToUnixEpochMilliseconds(this DateTime dt) {
            return Convert.ToInt64((dt.ToUniversalTime() - UnixEpoch).TotalMilliseconds);
        }

        /// <summary>
        /// Converts to a date time value assuming the value represents the number of
        /// milliseconds from the beginning of the Unix epoch.
        /// </summary>
        public static DateTime FromUnixEpochMilliseconds(this long timestamp) {
            return UnixEpoch.AddMilliseconds(timestamp);
        }

        /// <summary>
        /// Converts a date time value to seconds from the beginning of the Unix epoch.
        /// </summary>
        public static long ToUnixEpochSeconds(this DateTime dt) {
            return Convert.ToInt64((dt.ToUniversalTime() - UnixEpoch).TotalSeconds);
        }

        /// <summary>
        /// Converts to a date time value assuming the value represents the number of
        /// seconds from the beginning of the Unix epoch.
        /// </summary>
        public static DateTime FromUnixEpochSeconds(this long timestamp) {
            return UnixEpoch.AddSeconds(timestamp);
        }

        /// <summary>
        /// Gets the milliseconds between a date time value and now.
        /// </summary>
        public static long MillisecondsFromNow(this DateTime dt) {
            return Convert.ToInt64(dt.Subtract(DateTime.UtcNow).TotalMilliseconds);
        }

        /// <summary>
        /// Gets the milliseconds between a date time value and another.
        /// </summary>
        public static long MillisecondsFrom(this DateTime dt, DateTime target) {
            return Convert.ToInt64(target.Subtract(dt).TotalMilliseconds);
        }

    }

}

