using System;
using System.Collections.Generic;
using Urho;

namespace SmartRoadSense.Shared
{
    static class TerrainGenerator 
    {
        public static float TerrainStepLength = 0.5f;
        public static float TerrainBeginningOffset = -15.0f;
        public static float TerrainEndPoints = 3600;
        public static float PPECorrectionFactor = 20;
        public static float EndOfLevelSurfaceLength = 500;

        public static List<Point> ArrayToMatrix(List<float> srsNormalizedData)
        {
            List<Point> points = new List<Point>();
            uint idx = 0;
            foreach (var point in srsNormalizedData)
            {
                // Terrain point = collision chain index 
                // + terrain X coord progressive value (same distance each step)
                // + terrain Y coord from srs data
                points.Add(new Point(idx, TerrainStepLength * idx + TerrainBeginningOffset, point));
                idx++;
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

        public static List<float> RandomTerrain() 
        {
            var level = CharacterManager.Instance.User.Level;

            // TODO: modify difficulty based on user level

            List<float> data = new List<float>();
            for(var i = 0; i < TerrainEndPoints; i++)
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
