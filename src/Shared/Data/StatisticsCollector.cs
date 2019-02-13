using SmartRoadSense.Resources;
using SmartRoadSense.Shared.Database;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartRoadSense.Shared.Data {

    /// <summary>
    /// This class is responsible to calculate statistics 
    /// from simple datapieces and make them available to store.
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

        /// <summary>
        /// Multiplication factor for computed distance.
        /// </summary>
        /// <remarks>
        /// This should, primitively, account for the longer distances of curved roads
        /// that would be lost by computing only the distance of segments.
        /// </remarks>
        private const double DistanceFactor = 1.1;

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
                }
                else {
                    Log.Warning(new ArgumentException(nameof(piece.TrackId)), "Different track ID seen while collecting statistics");

                    CompleteSession();
                    Reset();
                }
            }

            _previous = piece;

            Log.Debug("Data piece collected");
        }

        public void Reset() {
            _previous = null;
            _tsStart = DateTime.MaxValue;
            _tsEnd = DateTime.MinValue;
            _ppeMax = Double.MinValue;
            _ppeAccumulator = 0.0;
            _ppeCount = 0;
            _ppeBins = new int[PpeMapper.BinCount];
            _distance = 0;
            _elapsed = TimeSpan.Zero;
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

            try {
                // TODO: perform in background
                using(var db = DatabaseUtility.OpenConnection()) {
                    var record = new StatisticRecord {
                        TrackId = _previous.TrackId,
                        Start = _tsStart,
                        End = _tsEnd,
                        MaxPpe = _ppeMax,
                        AvgPpe = (_ppeAccumulator / _ppeCount),
                        Bins = _ppeBins,
                        DistanceTraveled = _distance,
                        DataPieceCount = _ppeCount,
                        ElapsedTime = _elapsed
                    };

                    db.Insert(record);
                }
            }
            catch(Exception ex) {
                Log.Error(ex, "Failed to store statistics");
            }

            Log.Event("Recording.newStats", new Dictionary<string, string>() {
                { "distance", _distance.ToString(CultureInfo.InvariantCulture) },
                { "dataPieces", _ppeCount.ToString(CultureInfo.InvariantCulture) },
                { "elapsedMinutes", _elapsed.TotalMinutes.ToString(CultureInfo.InvariantCulture) }
            });

            UserLog.Add(LogStrings.StatsRecorded, _distance, _ppeCount);

            Reset();
        }

    }

}
