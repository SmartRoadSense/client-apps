using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartRoadSense.Shared;
using SmartRoadSense.Shared.Data;

namespace SmartRoadSense.Toolkit.Generators {

    internal class FileSequenceGenerator : BaseGenerator {

        public override IEnumerable<DataPiece> Generate() {
            if (Parameters.SourceFiles.IsEmpty()) {
                throw new ArgumentException("No source files for file sequence generator");
            }

            var json = new JsonSerializer();

            DateTime tsNew = DateTime.Now.Subtract(TimeSpan.FromDays(1));

            foreach (var f in Parameters.SourceFiles) {
                Program.VerboseLog("Processing file {0}...", f);

                using (var fs = new FileStream(f, FileMode.Open)) {
                    using (var reader = new JsonTextReader(new StreamReader(fs))) {
                        var points = json.Deserialize<List<DataPiece>>(reader);

                        foreach (var p in points) {
                            Program.Stats.AddInputPoint();
                            Program.Stats.AddTrackId(p.TrackId);

                            // Re-stamp points if needed
                            if(Parameters.UpdateTimestamps) {
                                p.StartTimestamp = tsNew;
                                tsNew = tsNew.Add(TimeSpan.FromSeconds(1));
                                p.EndTimestamp = tsNew;
                            }

                            yield return p;
                        }
                    }
                }
            }

            yield break;
        }

    }

}
