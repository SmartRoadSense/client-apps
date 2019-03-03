using SmartRoadSense.Resources;
using SmartRoadSense.Shared.Database;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SmartRoadSense.Shared.Data {

    /// <summary>
    /// This class takes care of recording statistics and full data dumps
    /// for recording sessions.
    /// </summary>
    public class DataWriter {

        private const int FlushIntervalSize = 120; // 2 minutes worth of data

        private DataPiece _previous;
        private StatisticRecord _record;
        private double _ppeAccumulator = 0.0;
        private uint _ppeCount = 0;

        private TextWriter _dumpWriter;

        /// <summary>
        /// Multiplication factor for computed distance.
        /// </summary>
        /// <remarks>
        /// This should, primitively, account for the longer distances of curved roads
        /// that would be lost by computing only the distance of segments.
        /// </remarks>
        private const double DistanceFactor = 1.005;

        public DataWriter() {
            Reset();
        }

        /// <summary>
        /// Collects a new piece of data.
        /// </summary>
        public void Collect(DataPiece piece) {
            if(_record != null && (_record.TrackId != piece.TrackId)) {
                Log.Warning(new ArgumentException(nameof(piece.TrackId)), "Different track ID seen while collecting statistics");

                CompleteSession();
            }

            if(_record == null) {
                _record = GetRecordForPiece(piece);
            }

            // Record set, update
            _ppeAccumulator += piece.Ppe;
            _ppeCount += 1;
            _record.Bins[PpeMapper.GetBinIndex(piece.Ppe)] += 1;
            if (piece.Ppe > _record.MaxPpe)
                _record.MaxPpe = piece.Ppe;
            _record.AvgPpe = _ppeAccumulator / _ppeCount;
            _record.DataPieceCount = (int)_ppeCount;
            _record.End = piece.EndTimestamp;
            _record.ElapsedTime += (piece.EndTimestamp - piece.StartTimestamp);
            _record.LocationEndLatitude = piece.Latitude;
            _record.LocationEndLongitude = piece.Longitude;

            if(_previous != null) {
                var traveledDistance = GeoHelper.DistanceBetweenPoints(
                    _previous.Latitude, _previous.Longitude,
                    piece.Latitude, piece.Longitude
                ) * DistanceFactor;
                _record.DistanceTraveled += traveledDistance;

                Log.Debug("Traveled {0:F3}km, count {1} {2:t}-{3:t}",
                    traveledDistance, _ppeCount, _record.Start, _record.End);
            }

            _previous = piece;

            // Flush every few minutes
            if(_ppeCount > 0 && (_ppeCount % FlushIntervalSize == 0)) {
                Flush();
            }

            // Dump data
            // Generates approximately 89 bytes per measurement (~313 KB/hour)
            if(_dumpWriter == null) {
                var outputFilepath = FileNaming.GetDataTrackFilepath(piece.TrackId);
                Log.Debug("Appending to file {0}", outputFilepath);

                var dumpStream = FileOperations.AppendFile(outputFilepath);
                _dumpWriter = new StreamWriter(dumpStream);
                _dumpWriter.WriteLine("# SRS Track {0:N} {1:u}", piece.TrackId, DateTime.Now);
                _dumpWriter.WriteLine("# Start,End,Lat,Lng,PPE,PPEx,PPEy,PPEz,Speed,Bearing,Accuracy");
            }
            _dumpWriter.WriteLine(
                string.Format(CultureInfo.InvariantCulture, "{0},{1},{2:F5},{3:F5},{4:F3},{5:F2},{6:F2},{7:F2},{8:F1},{9:D},{10:D}",
                    piece.StartTimestamp.Ticks,
                    piece.EndTimestamp.Ticks,
                    piece.Latitude,
                    piece.Longitude,
                    piece.Ppe,
                    piece.PpeX,
                    piece.PpeY,
                    piece.PpeZ,
                    piece.Speed,
                    (int)piece.Bearing,
                    piece.Accuracy
                )
            );

            Log.Debug("Data piece collected");
        }

        private StatisticRecord GetRecordForPiece(DataPiece piece) {
            using(var db = DatabaseUtility.OpenConnection()) {
                var m = db.GetMapping<StatisticRecord>();
                var existingRecord = (StatisticRecord)db.FindWithQuery(
                    m,
                    $"SELECT * FROM {m.TableName} WHERE {nameof(StatisticRecord.TrackId)} = ?",
                    piece.TrackId
                );

                if(existingRecord != null) {
                    Log.Debug("Found existing track in DB, resuming data collection");
                    return existingRecord;
                }
                else {
                    Log.Debug("Creating new track record in DB");
                    var r = new StatisticRecord {
                        TrackId = piece.TrackId,
                        Start = piece.StartTimestamp,
                        End = piece.EndTimestamp,
                        LocationStartLatitude = piece.Latitude,
                        LocationStartLongitude = piece.Longitude,
                        LocationEndLatitude = piece.Latitude,
                        LocationEndLongitude = piece.Longitude,
                        Vehicle = piece.Vehicle,
                        Anchorage = piece.Anchorage,
                        NumberOfPeople = piece.NumberOfPeople
                    };
                    db.Insert(r);
                    return r;
                }
            }
        }

        public void Reset() {
            _previous = null;
            _record = null;
            _ppeAccumulator = 0.0;
            _ppeCount = 0;

            if(_dumpWriter != null) {
                _dumpWriter.Dispose();
                _dumpWriter = null;
            }
        }

        /// <summary>
        /// Flush out data, keep recording session.
        /// </summary>
        public void Flush() {
            if(_dumpWriter != null) {
                _dumpWriter.Flush();
            }

            if(_record != null) {
                try {
                    // TODO: perform in background
                    using(var db = DatabaseUtility.OpenConnection()) {
                        db.Update(_record);
                    }
                }
                catch(Exception ex) {
                    Log.Error(ex, "Failed to store statistics");
                }
            }
        }

        /// <summary>
        /// Complete session and flush out data to database.
        /// </summary>
        public void CompleteSession() {
            if(_ppeCount == 0) {
                Log.Debug("Completing session of 0 data pieces, ignoring");
                return;
            }

            Log.Debug("Completing statistic collection session");

            Flush();

            Log.Event("Recording.newStats", new Dictionary<string, string>() {
                { "distance", _record.DistanceTraveled.ToString(CultureInfo.InvariantCulture) },
                { "dataPieces", _ppeCount.ToString(CultureInfo.InvariantCulture) },
                { "elapsedMinutes", _record.ElapsedTime.TotalMinutes.ToString(CultureInfo.InvariantCulture) }
            });

            UserLog.Add(LogStrings.StatsRecorded, _record.DistanceTraveled, _ppeCount);

            Reset();
        }

    }

}
