using System;
using System.Collections.Generic;
using Urho;
using System.Linq;

namespace SmartRoadSense.Shared
{
    static class TerrainGenerator 
    {
        public static float TerrainStepLength = 0.5f;
        public static float TerrainBeginningOffset = -15.0f;
        public static float PPECorrectionFactor = 20;
        public static float EndOfLevelSurfaceLength = 500;

        public static List<Point> ArrayToMatrix(List<float> srsNormalizedData, ScreenInfoRatio screenInfo, bool addEndingPoints = true)
        {
            List<Point> points = new List<Point>();
            uint idx = 0;
            foreach (var point in srsNormalizedData)
            {
                // Terrain point = collision chain index 
                // + terrain X coord progressive value (same distance each step)
                // + terrain Y coord from srs data
                points.Add(new Point(idx, TerrainStepLength * idx + TerrainBeginningOffset * screenInfo.XScreenRatio, point * screenInfo.YScreenRatio));
                idx++;
            }

            if(addEndingPoints) {
                var lastPoint = points.Last();
                for(var i = 0; i < EndOfLevelSurfaceLength; i++) {
                    points.Add(new Point(idx, TerrainStepLength * idx + TerrainBeginningOffset * screenInfo.XScreenRatio, lastPoint.Vector.Y * screenInfo.YScreenRatio));
                    idx++;
                }
            }

            return points;
        }

        public class Point {

            public uint Vertex { get; private set; }
            public Vector2 Vector { get; private set; }

            public Point(uint vx, float x, float y) {
                Vertex = vx;
                Vector = new Vector2(x, y);
            }
        }

        public static List<float> RandomTerrain(int trackLength) 
        {
            var level = CharacterManager.Instance.User.Level;

            // TODO: modify difficulty based on user level

            List<float> data = new List<float>();
            for(var i = 0; i < trackLength; i++)
                data.Add(i > 0 ? data[i-1] + NextRandom(-0.05f, 0.05f) : NextRandom(-0.05f, 0.05f));

            var endTrace = data[data.Count - 1];
            for(var i = 1; i < EndOfLevelSurfaceLength; i++)
                data.Add(endTrace);

            return data;
        }

        static Random random = new Random();
        public static float NextRandom(float min, float max) 
        {
            return (float)((random.NextDouble() * (max - min)) + min); 
        }
    }
}
