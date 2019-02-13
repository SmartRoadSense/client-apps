using System;
using System.Linq;
using SQLite;

namespace SmartRoadSense.Shared.Database {

    public static class StatisticHelper {

        /// <summary>
        /// Queries for the statistics for the last recorded track.
        /// </summary>
        /// <returns>
        /// Statistics for the last track or null, if none recorded.
        /// </returns>
        public static StatisticRecord GetLastTrack(SQLiteConnection conn) {
            var mapping = conn.GetMapping<StatisticRecord>();
            return conn.FindWithQuery<StatisticRecord>(string.Format(
                "SELECT * FROM `{0}` ORDER BY `{1}` DESC LIMIT 1",
                mapping.TableName,
                mapping.FindColumnWithPropertyName(nameof(StatisticRecord.Start)).Name
            ));
        }

        public class Summary {
            public double Distance { get; set; }
            public int Count { get; set; }
        }

        /// <summary>
        /// Computes summary statistics on tracks collected during a recent time period.
        /// </summary>
        public static Summary GetPeriodSummary(SQLiteConnection conn, StatisticPeriod period) {
            var mapping = conn.GetMapping<StatisticRecord>();
            return conn.Query<Summary>(string.Format(
                "SELECT SUM(`DistanceTraveled`) AS 'Distance', COUNT(*) AS 'Count' FROM {0} WHERE `{1}` > ?",
                mapping.TableName,
                mapping.FindColumnWithPropertyName(nameof(StatisticRecord.End)).Name
            ), GetPeriodStart(period)).First();
        }

        private static DateTime GetPeriodStart(StatisticPeriod period) {
            switch(period) {
                case StatisticPeriod.Week:
                    return DateTime.UtcNow.AddDays(-7);

                case StatisticPeriod.Month:
                    return DateTime.UtcNow.AddMonths(-1);

                case StatisticPeriod.Overall:
                default:
                    return DateTime.MinValue;
            }
        }

    }

}
