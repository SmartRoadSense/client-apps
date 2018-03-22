using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRoadSense.Toolkit {

    public class Statistics {

        private Stopwatch _watch = new Stopwatch();

        public void StartTime() {
            _watch.Restart();
        }

        public void StopTime() {
            _watch.Stop();
        }

        public long MeasuredMilliseconds {
            get {
                return _watch.ElapsedMilliseconds;
            }
        }

        private long _pointCount = 0;
        private long _pointOutputCount = 0;

        private HashSet<Guid> _trackIds = new HashSet<Guid>();

        private long _chunkCount = 0;

        public void AddInputPoint() {
            _pointCount++;
        }

        public void AddOutputPoint() {
            _pointOutputCount++;
        }

        public void AddTrackId(Guid id) {
            _trackIds.Add(id);
        }

        public void AddChunk() {
            _chunkCount++;
        }

        public override string ToString() {
            return string.Format("Processed {0} point(s), {1} track(s), in {2} chunk(s), {3} point(s) written, in {4} ms",
                _pointCount, _trackIds.Count, _chunkCount, _pointOutputCount, _watch.ElapsedMilliseconds);
        }

    }

}
