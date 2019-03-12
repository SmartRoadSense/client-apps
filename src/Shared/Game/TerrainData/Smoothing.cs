using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;

namespace SmartRoadSense.Shared {
    public static class Smoothing {

        static readonly int _initTrackLength = 25;
        static readonly Random _random = new Random();
        public static float NextRandom(float min, float max) { return (float)((_random.NextDouble() * (max - min)) + min); }

        // Genera un incremento random fra 7 e 11%
        static float GenerateMaxIncrement(bool positive) {
            var increment = 0.09f + NextRandom(-0.02f, 0.02f);

            return positive ? increment : increment * -1;
        }

        // Smussa l'elemento successivo (next) rispetto al valore del precedente (last)
        // Se la differenza fra i due elementi è sotto la soglia del 10% allora ritorna
        // semplicemente il valore dell'elemento next. In caso contrario, ritorna un 
        // incremento massimo del 10% dello stesso verso della differenza iniziale.
        static SmoothModel Smooth(float last, float next) {
            if(last.CompareTo(0) == 0)
                last = 0.001f;

            var increment = (next - last) / last;

            if(Math.Abs(increment) > 0.1f)
                increment = GenerateMaxIncrement(increment > 0);

            var new_val = last + last * increment;

            return new SmoothModel {
                Increment = increment,
                Value = new_val
            };
        }

        static public List<float> SmoothTrack(List<float> ppe, int userLevel) {
            var smoothed = new List<float>();
            for(var i = 0; i < _initTrackLength; i++)
                ppe.Insert(i, 0);

            var firstElement = true;
            foreach(var element in ppe) {
                if(firstElement) {
                    smoothed.Add(ppe[0] * 25);
                    firstElement = false;
                    continue;
                }

                var smooth = Smooth(smoothed[smoothed.Count - 1], element * 25);
                smoothed.Add(smooth.Value);
            }

            var window = WindowSize(userLevel);
            List<float> kernel = Enumerable.Repeat((1.0f / window), window).ToList();
            return Convolute(smoothed, kernel);
        }

        static List<float> Convolute(List<float> ppe, List<float> window) {
            var kLen = window.Count;
            var sLen = ppe.Count;
            List<float> result = new List<float>();

            for(var i = 0; i < kLen + sLen + 1; i++)
                result.Add(0);

            for(var n = 0; n < sLen + kLen + 1; n++) {
                int kmin, kmax;

                kmin = (n >= kLen - 1) ? n - (kLen - 1) : 0;
                kmax = (n < sLen - 1) ? n : sLen - 1;

                for(var k = kmin; k <= kmax; k++) {
                    result[n] += ppe[k] * window[n - k];
                }
            }
            return result;
        }

        static int WindowSize(int userLevel) {
            var min = 5;
            var max = 25;
            var usrLevelMax = 100;

            double result = ((double)userLevel / (double)usrLevelMax) * (max - min);
            return (int)Math.Round(max - result);    
        }

        public static List<float> TestPpeTrack() {
            IEnumerable<float> recs = new List<float>();

            try {
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(App));
                string[] resourceNames = assembly.GetManifestResourceNames();
                var fullname = (from r in resourceNames where r.EndsWith("track_game.csv", StringComparison.Ordinal) select r).FirstOrDefault();

                using(var s = assembly.GetManifestResourceStream(fullname)) {
                    using(var reader = new StreamReader(s)) {
                        var csv = new CsvReader(reader);
                        csv.Configuration.CultureInfo = System.Globalization.CultureInfo.InvariantCulture;
                        csv.Configuration.HasHeaderRecord = true;
                        var records = csv.GetRecords<float>();
                        recs = records.ToList();

                        var list = recs.ToList();
                        var endTrace = list[list.Count - 1];
                        for(var i = 1; i < TerrainGenerator.EndOfLevelSurfaceLength; i++)
                            list.Add(endTrace);
                        recs = list;
                    }
                }
            }
            catch(Exception e) {
                System.Diagnostics.Debug.WriteLine("Error decoding csv ppe file: " + e);
            }

            return recs.ToList();
        }
    }

    public class SmoothModel
    {
        public float Increment { get; set; }
        public float Value { get; set; }
    }
}
