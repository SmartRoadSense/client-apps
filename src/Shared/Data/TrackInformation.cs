using System;
using System.Collections.Generic;
using System.Text;

namespace SmartRoadSense.Shared.Data {

    public class TrackInformation {

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

        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the date on which the track was recorded.
        /// </summary>
        public DateTime RecordedOn { get; private set; }

        /// <summary>
        /// Gets the length of the recording.
        /// </summary>
        public TimeSpan RecordingLength { get; private set; }

        /// <summary>
        /// Gets the recorded distance (in kms).
        /// </summary>
        public double RecordingDistance { get; private set; }

    }

}
