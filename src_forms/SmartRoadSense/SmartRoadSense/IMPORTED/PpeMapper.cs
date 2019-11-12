using System;
using System.Collections.Generic;
using System.Text;

namespace SmartRoadSense {

    /// <summary>
    /// Maps PPE values to bins and colors.
    /// </summary>
    public static class PpeMapper {

        public const int BinCount = 6;

        /// <summary>
        /// Computes the bin index for a PPE value.
        /// Lower PPE values correspond to lower indices.
        /// </summary>
        public static int GetBinIndex(double ppe) {
#if DEBUG
            if (ppe < 0.0)
                throw new ArgumentException("PPE cannot be negative");
#endif

            if (ppe < 0.3)
                return 0;
            else if (ppe < 0.7)
                return 1;
            else if (ppe < 1.3)
                return 2;
            else if (ppe < 1.8)
                return 3;
            else if (ppe < 2.4)
                return 4;
            else
                return 5;
        }

    }

}
