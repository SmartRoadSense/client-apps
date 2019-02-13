using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using MoreLinq;
using SmartRoadSense.Shared.Data;
using SmartRoadSense.Toolkit.Generators;
using SmartRoadSense.Toolkit.Parameters;
using SmartRoadSense.Toolkit.Producers;

namespace SmartRoadSense.Toolkit {

    public static class Program {

        internal static CommonParameters Parameters { get; private set; }

        public static int Main(string[] args) {
            var result = Parser.Default.ParseArguments<
                CommonParameters,
                PayloadParameters,
                GeoJsonParameters,
                StoreParameters,
                UploadParameters,
                CompressionRunParameters,
                CsvParameters
            >(args);

            var toolResult = result.MapResult(
                (CompressionRunParameters p) => CompressionRun.Execute(p),
                (CommonParameters p) => { Parameters = p; return -1; },
                (errs) => 1
            );
            if (toolResult >= 0)
                return toolResult;

            try {
                var generator = BaseGenerator.Create(Parameters);
                if (generator == null) {
                    ErrorLog("No supported data generator specified (check parameters).");
                    return 1;
                }
                VerboseLog("Using generator: {0}.", generator.GetType().Name);

                var points = generator.Generate();

                if (Parameters.Skip.HasValue) {
                    if (Parameters.Skip.Value < 0) {
                        throw new ArgumentException("Parameter 'skip' must be positive");
                    }

                    VerboseLog("Skipping {0} data pieces.", Parameters.Skip.Value);
                    points = points.Skip(Parameters.Skip.Value);
                }

                if (Parameters.Every.HasValue) {
                    if (Parameters.Every.Value <= 1) {
                        throw new ArgumentException("Parameter 'every' must be greather than 1");
                    }

                    VerboseLog("Taking every {0}-th data piece.", Parameters.Every.Value);
                    points = points.TakeEvery(Parameters.Every.Value);
                }

                if (Parameters.Length.HasValue) {
                    if (Parameters.Length.Value < 0) {
                        throw new ArgumentException("Parameter 'length' must be positive");
                    }

                    VerboseLog("Trimming point sequence to {0} data pieces.", Parameters.Length.Value);
                    points = points.Take(Parameters.Length.Value);
                }

                if(Parameters.OneTrackId && Parameters.AmendTrackId.HasValue) {
                    throw new ArgumentException("Cannot use track-id and one-track parameters at the same time");
                }
                if(Parameters.OneTrackId || Parameters.AmendTrackId.HasValue) {
                    Guid amendedTrackId = (Parameters.OneTrackId) ? points.First().TrackId : Parameters.AmendTrackId.Value;
                    points = points.Select(p => {
                        p.TrackId = amendedTrackId;
                        return p;
                    });
                }

                //Add output points counting hook
                points = points.Pipe(piece => Stats.AddOutputPoint());

                IEnumerable<IEnumerable<DataPiece>> chunks;
                if (Parameters.Chunking.HasValue) {
                    VerboseLog("Chunking point sequence into {0}-piece chunks.", Parameters.Chunking.Value);
                    chunks = points.Batch(Parameters.Chunking.Value);
                }
                else {
                    chunks = new IEnumerable<DataPiece>[] { points };
                }

                IProducer producer = result.MapResult(
                    (PayloadParameters opts) => (IProducer)new PayloadProducer(opts),
                    (StoreParameters opts) => (IProducer)new StoreProducer(opts),
                    (GeoJsonParameters opts) => (IProducer)new GeoJsonProducer(opts),
                    (UploadParameters opts) => (IProducer)new UploadProducer(opts),
                    (CsvParameters opts) => (IProducer)new CsvProducer(opts),
                    (errs) => null
                );
                if (producer == null) {
                    ErrorLog("No supported data producer specified (check parameters).");
                    return 1;
                }
                VerboseLog("Using producer: {0}.", producer.GetType().Name);

                Stats.StartTime();

                producer.Process(chunks.ToArray());

                Stats.StopTime();

                VerboseLog("Production terminated.");
            }
            catch(AggregateException aggEx) {
                int i = 1;
                foreach(var ex in aggEx.InnerExceptions) {
                    ErrorLog(" === Error #{0} ===", i++);
                    ExceptionLog(ex);
                }

                return 1;
            }
            catch (Exception ex) {
                ExceptionLog(ex);

                return 1;
            }

            Log(Stats.ToString());

            return 0;
        }

        internal static Statistics Stats = new Statistics();

        internal static void Log(string format, params object[] parms) {
            Console.Error.WriteLine(format, parms);
        }

        internal static void ErrorLog(string format, params object[] parms) {
            Console.Error.WriteLine(format, parms);
        }

        internal static void ExceptionLog(Exception ex) {
            ErrorLog("Error ({1}): {0}", ex.Message, ex.GetType().Name);
            ErrorLog("Stacktrace:");
            ErrorLog(ex.StackTrace);
        }

        internal static void VerboseLog(string format, params object[] parms) {
            if (Parameters.Verbose) {
                Console.Error.WriteLine(format, parms);
            }
        }

    }

}
