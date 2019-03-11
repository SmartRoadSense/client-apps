using System;
namespace SmartRoadSense.Shared {

    public class Stopwatch {

        public DateTime StartTime { get; private set; }
        TimeSpan _elapsedTime { get; set; }
        bool _paused;

        public Stopwatch() {
            StartTime = DateTime.Now;
            _elapsedTime = TimeSpan.Zero;
        }

        public void Pause() {
            _paused = true;
            _elapsedTime += DateTime.Now.Subtract(StartTime);
        }

        public void Resume() {
            _paused = false;
            StartTime = DateTime.Now;
        }

        public TimeSpan GetElapsedTime() {
            if(!_paused) {
                var elapsed = new TimeSpan(DateTime.Now.Ticks - StartTime.Ticks) + _elapsedTime;
                return elapsed;
            }
            return _elapsedTime;
        }
    }

    public static class StopwatchExtension {

        public static string MillisRepresentation(this TimeSpan timeSpan) {
            return string.Format("{0:mm\\:ss\\.fff}", timeSpan);
        }
    }
}
