using SmartRoadSense.Resources;
using SmartRoadSense.Shared;
using SmartRoadSense.Shared.Database;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace SmartRoadSense.Shared.Data {

    /// <summary>
    /// This class takes care of recording statistics and full data dumps
    /// for recording sessions.
    /// </summary>
    public class StatisticsCollector {

        private DataPiece _previous;
        private DateTime _tsStart, _tsEnd;
        private double _ppeMax;
        private double _ppeAccumulator;
        private int _ppeCount;
        private int[] _ppeBins;
        private double _distance;
        private TimeSpan _elapsed;

        private TextWriter _dumpWriter;

        /// <summary>
        /// Multiplication factor for computed distance.
        /// </summary>
        /// <remarks>
        /// This should, primitively, account for the longer distances of curved roads
        /// that would be lost by computing only the distance of segments.
        /// </remarks>
        private const double DistanceFactor = 1.005;

        public StatisticsCollector() {
            Reset();
        }

        /// <summary>
        /// Collects a new piece of data.
        /// </summary>
        public void Collect(DataPiece piece) {
            if(_previous != null) {
                if (_previous.TrackId == piece.TrackId) {
                    _ppeAccumulator += piece.Ppe;
                    _ppeCount += 1;
                    _ppeBins[PpeMapper.GetBinIndex(piece.Ppe)] += 1;
                    if (piece.Ppe > _ppeMax)
                        _ppeMax = piece.Ppe;

                    if (piece.StartTimestamp < _tsStart)
                        _tsStart = piece.StartTimestamp;
                    if (piece.EndTimestamp > _tsEnd)
                        _tsEnd = piece.EndTimestamp;
                    _elapsed += (piece.EndTimestamp - piece.StartTimestamp);

                    var traveledDistance = GeoHelper.DistanceBetweenPoints(
                        _previous.Latitude, _previous.Longitude,
                        piece.Latitude, piece.Longitude
                    ) * DistanceFactor;
                    _distance += traveledDistance;

                    Log.Debug("Traveled {0:F3}km, count {1} {2:t}-{3:t}", traveledDistance, _ppeCount, _tsStart, _tsEnd);

                    // Flush every few minutes
                    if(_ppeCount > 0 && (_ppeCount % 120 == 0)) {
                        Flush();
                    }
                }
                else {
                    Log.Warning(new ArgumentException(nameof(piece.TrackId)), "Different track ID seen while collecting statistics");

                    CompleteSession();
                }
            }

            // Dump data
            // Generates approximately 43 bytes per measurement (~151 KB/hour)
            if(_dumpWriter == null) {
                var dumpStream = FileOperations.AppendFile(FileNaming.GetDataTrackFilepath(piece.TrackId));
                _dumpWriter = new StreamWriter(dumpStream);
            }
            _dumpWriter.WriteLine(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0:s},{1:F5},{2:F5},{3:F2}",
                    piece.StartTimestamp,
                    piece.Latitude,
                    piece.Longitude,
                    piece.Ppe
                )
            );

            _previous = piece;

            Log.Debug("Data piece collected");
        }

        public void Reset() {
            _previous = null;
            _tsStart = DateTime.MaxValue;
            _tsEnd = DateTime.MinValue;
            _ppeMax = double.MinValue;
            _ppeAccumulator = 0.0;
            _ppeCount = 0;
            _ppeBins = new int[PpeMapper.BinCount];
            _distance = 0;
            _elapsed = TimeSpan.Zero;

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

            Task.Run(() => {
                try {
                    using(var db = DatabaseUtility.OpenConnection()) {
                        var mapping = db.GetMapping<StatisticRecord>();
                        var record = db.FindWithQuery<StatisticRecord>(string.Format(
                            "SELECT * FROM `{0}` WHERE `{1}` = ?",
                            mapping.TableName,
                            mapping.FindColumnWithPropertyName(nameof(StatisticRecord.TrackId)).Name
                        ), _previous.TrackId) ?? new StatisticRecord {
                            TrackId = _previous.TrackId
                        };

                        record.Start = _tsStart;
                        record.End = _tsEnd;
                        record.MaxPpe = _ppeMax;
                        record.AvgPpe = (_ppeAccumulator / _ppeCount);
                        record.Bins = _ppeBins;
                        record.DistanceTraveled = _distance;
                        record.DataPieceCount = _ppeCount;
                        record.ElapsedTime = _elapsed;

                        db.InsertOrReplace(record);
                    }
                }
                catch(Exception ex) {
                    Log.Error(ex, "Failed to store statistics");
                }
            });
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
                { "distance", _distance.ToUnits() },
                { "dataPieces", _ppeCount.ToUnits() }
            });

            UserLog.Add(LogStrings.StatsRecorded, _distance, _ppeCount);

            Reset();
        }

    }

}
