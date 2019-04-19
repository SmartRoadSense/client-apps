using System;
namespace SmartRoadSense.Shared {
    public class GameDistanceData {
        public float Start { get; private set; }
        public float End { get; private set; }

        public GameDistanceData(float startX, float endX) {
            Start = startX;
            End = endX;
        }

        public int Distance(float currentPosition, int terrainLength) {
            int x = currentPosition < Start
                ? 0
                : currentPosition > End
                    ? terrainLength
                    : (int)(currentPosition * (1 / TerrainGenerator.TerrainStepLength));
            return x;
        }

        public string DistanceText(float currentPosition, int terrainLength) {
            var x = currentPosition < Start
                ? 0
                : currentPosition > End
                    ? terrainLength
                    : (int)currentPosition * (1 / TerrainGenerator.TerrainStepLength);
            return string.Format("{0} m", x);
        }
    }
}
