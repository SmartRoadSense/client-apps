using System;
using Urho;

namespace SmartRoadSense.Shared {

    public class MapPositionData {
        public float Start { get; private set; }
        public float End { get; private set; }
        public float MapBoxStart { get; private set; }
        public float MapBoxEnd { get; private set; }
        public float MapBoxYStart { get; private set; }
        public float MapBoxYEnd { get; private set; }

        float _ratioX;
        float _ratioY;

        public MapPositionData(float startX, float endX) {
            Start = startX;
            End = endX;
            MapBoxEnd = 0;
        }

        public void Initialize (float mapBoxStartX, float mapBoxEndX, float mapBoxStartY, float mapBoxEndY, float ratioX, float ratioY)
        {
            MapBoxStart = mapBoxStartX;
            MapBoxEnd = mapBoxEndX;
            MapBoxYStart = mapBoxStartY;
            MapBoxYEnd = mapBoxEndY;

            _ratioX = ratioX;
            _ratioY = ratioY;
        }

        public int LocatorPosition(float currentPosition) {
                if(MapBoxEnd.Equals(0))
                    throw new Exception("Initialize has not been launched");

                var x = currentPosition < Start
                    ? MapBoxStart
                    : currentPosition > End
                        ? MapBoxEnd
                        : ((1 / TerrainGenerator.TerrainStepLength) * (currentPosition * MapBoxEnd) / TerrainGenerator.TerrainEndPoints * _ratioX) + MapBoxStart;
                return (int)x;
        }

        public Vector2 TerrainPoint(Vector2 currentPosition) {
            if(MapBoxEnd.Equals(0))
                throw new Exception("Initialize has not been launched");

            var x = currentPosition.X < Start
                ? MapBoxStart
                : currentPosition.X > End
                    ? MapBoxEnd
                    : ((1 / TerrainGenerator.TerrainStepLength) * (currentPosition.X * MapBoxEnd) / TerrainGenerator.TerrainEndPoints * _ratioX) + MapBoxStart;

            var y = currentPosition.Y < 0
                ? MapBoxStart
                :  (currentPosition.Y / MapBoxYEnd * _ratioY) + MapBoxYStart;

            return new Vector2(x, y);
        }
    }
}
