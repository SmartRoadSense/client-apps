using System;
using System.Collections.Generic;
using SQLite;

namespace SmartRoadSense.Shared.Database {

    public static class DatabaseQueries {

        public class TrackAndCount {
            public Guid TrackId { get; set; }
            public int DataCount { get; set; }
            public int UploadedCount { get; set; }
            public VehicleType VehicleType { get; set; }
            public AnchorageType AnchorageType { get; set; }
            public int NumberOfPeople { get; set; }
        }

        public static IList<TrackAndCount> GetTracksAndUploadedCount(this SQLiteConnection c, int offset = 0, int count = 10) {
            return c.Query<TrackAndCount>(
                @"SELECT StatisticRecord.TrackId AS TrackId, StatisticRecord.DataPieceCount AS DataCount, SUM(TrackUploadRecord.Count) AS UploadedCount,
                  StatisticRecord.Vehicle AS VehicleType, StatisticRecord.Anchorage AS AnchorageType, StatisticRecord.NumberOfPeople AS NumberOfPeople
                  FROM StatisticRecord LEFT OUTER JOIN TrackUploadRecord ON StatisticRecord.TrackId = TrackUploadRecord.TrackId
                  GROUP BY StatisticRecord.TrackId
                  ORDER BY StatisticRecord.Start DESC
                  LIMIT ? OFFSET ?",
                count, offset);
        }

        public static IList<TrackAndCount> GetAllPendingTracks(this SQLiteConnection c) {
            return c.Query<TrackAndCount>(
                @"SELECT StatisticRecord.TrackId AS TrackId, StatisticRecord.DataPieceCount AS DataCount, SUM(TrackUploadRecord.Count) AS UploadedCount,
                  StatisticRecord.Vehicle AS VehicleType, StatisticRecord.Anchorage AS AnchorageType, StatisticRecord.NumberOfPeople AS NumberOfPeople
                  FROM StatisticRecord LEFT OUTER JOIN TrackUploadRecord ON StatisticRecord.TrackId = TrackUploadRecord.TrackId
                  GROUP BY StatisticRecord.TrackId
                  HAVING UploadedCount IS NULL OR DataCount > UploadedCount
                  ORDER BY StatisticRecord.Start DESC"
            );
        }

        public static IList<StatisticRecord> GetTracks(this SQLiteConnection c, TimeOrdering ordering = TimeOrdering.OldFirst) {
            var m = c.GetMapping<StatisticRecord>();
            string orderKeyword = (ordering == TimeOrdering.OldFirst) ? "ASC" : "DESC";
            return c.Query<StatisticRecord>(
                $"SELECT * FROM {m.TableName} ORDER BY {nameof(StatisticRecord.Start)} {orderKeyword}"
            );
        }

    }

}
