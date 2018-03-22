﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartRoadSense.Shared {

    /// <summary>
    /// User-visible log.
    /// </summary>
    public static class UserLog {

        /// <summary>
        /// Special icon marker on log entries.
        /// </summary>
        public enum Icon {
            None,
            Warning,
            Error
        }

        /// <summary>
        /// Represents a log entry.
        /// </summary>
        [JsonObject(MemberSerialization.OptIn)]
        #if __ANDROID__
        [global::Android.Runtime.Preserve(AllMembers = true)]
        #endif
        public class LogEntry {

            public LogEntry() {
            }

            public LogEntry(Icon icon, DateTime timestamp, string message) {
                Icon = icon;
                TimestampUtc = timestamp;
                Message = message;
            }

            [JsonProperty]
            public Icon Icon { get; private set; }

            [JsonProperty("Timestamp")]
            private DateTime TimestampUtc { get; set; }

            public DateTime Timestamp {
                get {
                    return TimestampUtc.ToLocalTime();
                }
            }

            [JsonProperty]
            public string Message { get; private set; }

        }

        /// <summary>
        /// Initialize the log from disk.
        /// </summary>
        /// <remarks>
        /// Can be awaited on synchronously (does not capture context).
        /// </remarks>
        public static async Task Initialize() {
            try {
                using(var fileStream = await FileOperations.ReadFile(FileNaming.LogStorePath)) {
                    await Task.Run(() => {
                        JsonSerializer serializer = new JsonSerializer();

                        using (var textReader = new StreamReader(fileStream)) {
                            var data = serializer.Deserialize<LogEntry[]>(textReader);

                            if (data == null || data.Length == 0)
                                return;

                            lock (_rootLock) {
                                _log = new Queue<LogEntry>(data);
                            }
                        }
                    }).ConfigureAwait(false);
                }
            }
            catch(FileNotFoundException) {
                Log.Debug("No user log file found to deserialize (at {0})", FileNaming.LogStorePath);
                return;
            }
            catch(Exception ex) {
                Log.Error(ex, "Failed to deserialize log from file store (at {0})", FileNaming.LogStorePath);
                return;
            }
        }

        /// <summary>
        /// Persist the log to disk.
        /// </summary>
        /// <remarks>
        /// Can be awaited on synchronously (does not capture context).
        /// </remarks>
        public static async Task Persist() {
            LogEntry[] data = null;
            lock (_rootLock) {
                data = _log.ToArray();
            }

            try {
                using(var fileStream = await FileOperations.CreateOrTruncateFile(FileNaming.LogStorePath)) {
                    await Task.Run(() => {
                        using (var textWriter = new StreamWriter(fileStream)) {
                            JsonSerializer serializer = new JsonSerializer();
                            serializer.Serialize<LogEntry[]>(textWriter, data);
                        }

                    }).ConfigureAwait(false);
                }
            }
            catch(Exception ex) {
                Log.Error(ex, "Failed to persist log to store");
            }

            Log.Debug("Log persisted");
        }

        public const int MaximumLogSize = 100;

        private static object _rootLock = new object();

        private static Queue<LogEntry> _log = new Queue<LogEntry>(MaximumLogSize);

        /// <summary>
        /// Adds an iconless message to the log.
        /// </summary>
        public static void Add(string format, params object[] args) {
            Add(Icon.None, format, args);
        }

        /// <summary>
        /// Adds a message with an icon to the log.
        /// </summary>
        public static void Add(Icon icon, string format, params object[] args) {
            var entry = new LogEntry(icon, DateTime.UtcNow, string.Format(format, args));

            lock (_rootLock) {
                _log.Enqueue(entry);
            }

            OnNewEntryAdded(entry);
        }

        /// <summary>
        /// Gets the entries of the log.
        /// </summary>
        public static IReadOnlyList<LogEntry> Entries {
            get {
                IReadOnlyList<LogEntry> ret = null;
                lock (_rootLock) {
                    ret = _log.ToArray();
                }

                return ret;
            }
        }

        public static void Clear() {
            lock (_rootLock) {
                _log.Clear();
            }
        }

        /// <summary>
        /// Gets the count of entries of the log.
        /// </summary>
        public static int Count {
            get {
                return _log.Count;
            }
        }

        #region Events

        public class NewEntryEventArgs : EventArgs {

            public NewEntryEventArgs(LogEntry entry) {
                Entry = entry;
            }

            public LogEntry Entry { get; private set; }

        }

        /// <summary>
        /// Occurs when new entry is added.
        /// </summary>
        public static event EventHandler<NewEntryEventArgs> NewEntryAdded;

        private static void OnNewEntryAdded(LogEntry entry) {
            var evt = NewEntryAdded;
            if (evt != null) {
                evt(null, new NewEntryEventArgs(entry));
            }
        }

        #endregion

    }

}

