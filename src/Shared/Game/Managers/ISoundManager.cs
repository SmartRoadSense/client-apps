using System;
namespace SmartRoadSense.Shared {
    public interface ISoundManager {
        float MusicGain { get; set; }
        float EffectsGain { get; set; }
    }
}
