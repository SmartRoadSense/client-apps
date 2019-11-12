using System;

namespace SmartRoadSense {

    public enum SyncPolicy {
        /// <summary>
        /// Default synchronization policy.
        /// </summary>
        Default,
        /// <summary>
        /// Force synchronization attempt.
        /// </summary>
        Forced,
        /// <summary>
        /// Force only most recent file to be synchronized.
        /// </summary>
        ForceLast
    }

}
