using System;
using System.Collections.Generic;
using System.Text;

namespace SmartRoadSense.Shared.Data {

    public readonly struct DataLine {

        public DataLine(long startTicks, long endTicks, double latitude, double longitude, double ppe, double ppeX, double ppeY, double ppeZ, double speed, int bearing, int accuracy) {
            StartTicks = startTicks;
            EndTicks = endTicks;
            Latitude = latitude;
            Longitude = longitude;
            Ppe = ppe;
            PpeX = ppeX;
            PpeY = ppeY;
            PpeZ = ppeZ;
            Speed = speed;
            Bearing = bearing;
            Accuracy = accuracy;
        }

        public readonly long StartTicks;
        public readonly long EndTicks;
        public readonly double Latitude;
        public readonly double Longitude;
        public readonly double Ppe;
        public readonly double PpeX, PpeY, PpeZ;
        public readonly double Speed;
        public readonly int Bearing;
        public readonly int Accuracy;

    }

}
