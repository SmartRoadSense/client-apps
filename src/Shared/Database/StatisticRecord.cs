using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace SmartRoadSense.Shared.Database {

    /// <summary>
    /// Represents a record with statistics about a recorded track.
    /// </summary>
#if __ANDROID__
    [global::Android.Runtime.Preserve(AllMembers = true)]
#endif
    public class StatisticRecord {

        public StatisticRecord() {
            _bins = new int[PpeMapper.BinCount];
        }

        /// <summary>
        /// Gets or sets the synthetic ID used by this local installation.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the recording session (i.e., the track).
        /// </summary>
        [Indexed, NotNull]
        public Guid TrackId { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public double MaxPpe { get; set; }

        public double AvgPpe { get; set; }

        /// <summary>
        /// Distance traveled in meters.
        /// </summary>
        public double DistanceTraveled { get; set; }

        /// <summary>
        /// Elapsed time while recording.
        /// </summary>
        public TimeSpan ElapsedTime { get; set; }

        private int[] _bins;

        [Ignore]
        public int[] Bins {
            get {
                return _bins;
            }
            set {
                if (value == null)
                    throw new ArgumentNullException();
                if (value.Length != PpeMapper.BinCount)
                    throw new ArgumentOutOfRangeException();

                _bins = value;
            }
        }

        public int Bin1Count {
            get { return _bins[0]; }
            set { _bins[0] = value; }
        }

        public int Bin2Count {
            get { return _bins[1]; }
            set { _bins[1] = value; }
        }

        public int Bin3Count {
            get { return _bins[2]; }
            set { _bins[2] = value; }
        }

        public int Bin4Count {
            get { return _bins[3]; }
            set { _bins[3] = value; }
        }

        public int Bin5Count {
            get { return _bins[4]; }
            set { _bins[4] = value; }
        }

        public int Bin6Count {
            get { return _bins[5]; }
            set { _bins[5] = value; }
        }
        
        /// <summary>
        /// Number of data pieces this statistic is based on.
        /// </summary>
        public int DataPieceCount { get; set; }

    }

}
