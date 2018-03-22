using System;

namespace SmartRoadSense.Shared {
    
    public static class PpeColorMapper {

        private static PlatformColor[] _colors= {
            new PlatformColor(0  , 128, 0),
            new PlatformColor(138, 205, 0),
            new PlatformColor(157, 215, 0),
            new PlatformColor(201, 236, 0),
            new PlatformColor(255, 255, 0),
            new PlatformColor(255, 215, 0),
            new PlatformColor(255, 184, 0),
            new PlatformColor(255, 160, 0),
            new PlatformColor(255, 140, 0),
            new PlatformColor(255,   0, 0)
        };

        private static double[] _thresholds = {
            0.0811,
            0.2058,
            0.3824,
            0.5861,
            0.8311,
            1.1207,
            1.4040,
            1.6392,
            1.7985
        };

        static PpeColorMapper() {
            #if DEBUG
            if(_colors.Length != _thresholds.Length + 1) {
                throw new InvalidOperationException("Invalid threshold / colors length");
            }
            #endif
        }

        /// <summary>
        /// Gets the matching color for a given PPE value.
        /// </summary>
        public static PlatformColor Map(double ppe) {
            for(int i = 0; i < _thresholds.Length; ++i) {
                if(ppe <= _thresholds[i]) {
                    return _colors[i];
                }
            }

            return _colors[_colors.Length - 1];
        }

    }

}

