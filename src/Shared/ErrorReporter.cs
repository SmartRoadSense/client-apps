using System;
using SmartRoadSense.Core;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SmartRoadSense.Shared {

#if !WINDOWS_PHONE_APP

    /// <summary>
    /// Handles internal errors and reporting.
    /// </summary>
    public static class ErrorReporter {

        /// <summary>
        /// Takes a full dump of the internal error and writes it to file.
        /// </summary>
        public static Task ExecuteDump(EngineComputationException error) {
            return Task.Run(async () => {
                try {
                    using(var fs = await FileOperations.CreateOrTruncateFile(FileNaming.ErrorDumpPath)) {
                        using (var writer = new StreamWriter(fs)) {
                            writer.WriteLine(App.ApplicationInformation);
                            writer.WriteLine("Engine dump recorded on {0:u}", DateTime.UtcNow);
                            writer.WriteLine();
                            WriteBuffer(writer, "Primary buffer", error.PrimaryBufferSnapshot);
                            writer.WriteLine();
                            WriteBuffer(writer, "Secondary buffer", error.SecondaryBufferSnapshot);
                        }
                    }
                }
                catch(Exception ex) {
                    Log.Error(ex, "Failed while writing engine dump");
                }
            });
        }

        private static void WriteBuffer(TextWriter writer, string title, BufferSnapshot buffer) {
            writer.WriteLine(title);

            writer.WriteLine("TS,AccX,AccY,AccZ,Lat,Lng,Speed,Bearing,Acc,");
            for (int i = 0; i < buffer.Size; ++i) {
                writer.Write("{0},", buffer.Timestamps[i]);
                writer.Write("{0},", buffer.AccelerationsX[i]);
                writer.Write("{0},", buffer.AccelerationsY[i]);
                writer.Write("{0},", buffer.AccelerationsZ[i]);
                writer.Write("{0},", buffer.Latitudes[i]);
                writer.Write("{0},", buffer.Longitudes[i]);
                writer.Write("{0},", buffer.Speeds[i]);
                writer.Write("{0},", buffer.Bearings[i]);
                writer.Write("{0},", buffer.Accuracies[i]);
                writer.WriteLine();
            }
            writer.WriteLine("Fill index: {0}, Prefill size {1}", buffer.FillIndex, buffer.PrefillSize);
        }

        /// <summary>
        /// Gets whether a dump is available to be reported.
        /// </summary>
        public static bool HasDump() {
            return File.Exists(FileNaming.ErrorDumpPath);
        }

        /// <summary>
        /// Reads the dump file and represents it in a string that can be reported.
        /// The dump file is then deleted.
        /// </summary>
        public async static Task<string> ReadDumpAndDelete() {
            if (!File.Exists(FileNaming.ErrorDumpPath)) {
                Log.Debug("Attempting to read and delete non existing dump file");
                return string.Empty;
            }

            string dump = string.Empty;
            try {
                await Task.Run(() => {
                    using(var fs = new FileStream(FileNaming.ErrorDumpPath, FileMode.Open, FileAccess.Read)) {
                        using (var reader = new StreamReader(fs)) {
                            dump = reader.ReadToEnd();
                        }
                    }

                    File.Delete(FileNaming.ErrorDumpPath);
                });

                return dump;
            }
            catch(Exception ex) {
                Log.Error(ex, "Failed reading error dump");
                return string.Empty;
            }
        }

        /// <summary>
        /// Drops the dump file.
        /// </summary>
        public static Task DropDump() {
            return Task.Run(() => {
                try {
                    File.Delete(FileNaming.ErrorDumpPath);
                }
                catch(Exception ex) {
                    Log.Error(ex, "Failed to delete dump file");
                }
            });

        }

    }

#endif

}

