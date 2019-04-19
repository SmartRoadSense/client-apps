using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartRoadSense.Shared.Database;
using SmartRoadSense.Shared.DataModel;

namespace SmartRoadSense.Shared.Data {

    /// <summary>
    /// Data management, parsing and conversion functions.
    /// </summary>
    public static class DataStore {

        public static Task<IList<TrackInformation>> GetCollectedTracks(TimeOrdering ordering) {
            return Task.Run<IList<TrackInformation>>(() => {
                using(var db = DatabaseUtility.OpenConnection()) {
                    var records = db.GetTracks(ordering);

                    return (from r in records
                            select new TrackInformation(
                                r.TrackId,
                                r.Start,
                                r.ElapsedTime,
                                r.DistanceTraveled
                            )).ToList();
                }
            });
        }

        public static Task<int> GetTrackLength(Guid trackId) {
            return Task.Run(async () => {
                var reader = new DataReader(trackId);
                int length = 0;
                while(await reader.Advance()) {
                    length++;
                }
                return length;
            });
        }

        public static Task<double[]> GetTrackPpe(Guid trackId) {
            return Task.Run(async () => {
                var reader = new DataReader(trackId);
                List<double> data = new List<double>();
                while(await reader.Advance()) {
                    data.Add(reader.Current.Ppe);
                }
                return data.ToArray();
            });
        }

        /// <summary>
        /// Deletes all queued data files.
        /// </summary>
        public static Task DeleteAll() {
            // TODO: this actually does nothing now
            // Makes no sense to drop queued data files as all data is permanently stored
            return Task.CompletedTask;
        }

    }

}
