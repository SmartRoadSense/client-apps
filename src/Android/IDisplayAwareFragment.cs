using System;

namespace SmartRoadSense.Android {

    /// <summary>
    /// Interface for fragments that need to be aware about their display status.
    /// </summary>
    public interface IDisplayAwareFragment {

        /// <summary>
        /// Called when the fragment is shown.
        /// </summary>
        void Shown();

        /// <summary>
        /// Called when the fragment is hidden.
        /// </summary>
        void Hidden();

    }

}

