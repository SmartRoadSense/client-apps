using System;
using Newtonsoft.Json;

namespace SmartRoadSense {

    /// <summary>
    /// POCO data structure that holds information about a recorded data point
    /// as stored on disk in JSON format.
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    #if __ANDROID__
    [global::Android.Runtime.Preserve(AllMembers = true)]
    #endif
    public class DataPiece {

        public Guid TrackId;

        public DateTime StartTimestamp;
        public DateTime EndTimestamp;

        public double Ppe;
        public double PpeX, PpeY, PpeZ;
        public double Speed;

        public double Latitude, Longitude;

        public float Bearing;
        public int Accuracy;

        // TODO: these values should be moved outside, since they should never change
        //       inside a recording session
        public VehicleType Vehicle;
        public AnchorageType Anchorage;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [System.ComponentModel.DefaultValue(-1)]
        public int NumberOfPeople;

    }

}
