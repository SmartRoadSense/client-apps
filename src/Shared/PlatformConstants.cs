using System;
using System.Collections.Generic;
using System.Text;

namespace SmartRoadSense.Shared {

    public static class PlatformConstants {

        public static StringComparison InvariantStringComparison {
            get {
#if WINDOWS_PHONE_APP
                return StringComparison.OrdinalIgnoreCase;
#else
                return StringComparison.InvariantCultureIgnoreCase;
#endif
            }
        }

    }

}
