﻿using System;
using SQLite;

namespace SmartRoadSense {

    /// <summary>
    /// Represents a record about an uploaded track chunk.
    /// </summary>
#if __ANDROID__
    [global::Android.Runtime.Preserve(AllMembers = true)]
#endif
    public class TrackUploadRecord {

        /// <summary>
        /// Gets or sets the synthetic ID used by this local installation.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the original recorded track.
        /// Uploaded chunks from the same track have been generated during the same recording session.
        /// </summary>
        [Indexed, NotNull]
        public Guid TrackId { get; set; }

        /// <summary>
        /// Gets or sets the unique ID generated by the online service.
        /// </summary>
        [Indexed]
        public int UploadedId { get; set; }

        /// <summary>
        /// Gets or sets the secret generated for this uploaded chunk.
        /// </summary>
        public byte[] Secret { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of when the upload was completed (in UTC).
        /// </summary>
        public DateTime UploadedOn { get; set; }

    }

}
