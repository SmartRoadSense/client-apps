using System;
using System.Collections.Generic;
using System.Text;

namespace SmartRoadSense.Shared.Data {

    public struct TrackInformation {

        public TrackInformation(
            Guid id,
            DateTime startTimestamp,
            TimeSpan elapsedTime,
            double distance
        ) {
            Id = id;
            RecordedOn = startTimestamp;
            RecordingLength = elapsedTime;
            RecordingDistance = distance;
        }

        public readonly Guid Id;

        /// <summary>
        /// Gets the date on which the track was recorded.
        /// </summary>
        public readonly DateTime RecordedOn;

        /// <summary>
        /// Gets the length of the recording.
        /// </summary>
        public readonly TimeSpan RecordingLength;

        /// <summary>
        /// Gets the recorded distance (in kms).
        /// </summary>
        public readonly double RecordingDistance;

    }

}
