using System;

namespace SmartRoadSense {

    /// <summary>
    /// Extension methods for events.
    /// </summary>
    public static class EventExtensions {

        /// <summary>
        /// Safely raises an event.
        /// </summary>
        public static void Raise(this EventHandler handler, object sender) {
            var evt = handler;
            if (evt != null) {
                evt.Invoke(sender, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Safely raises an event.
        /// </summary>
        public static void Raise<T>(this EventHandler<T> handler, object sender, T args) {
            var evt = handler;
            if (evt != null) {
                evt.Invoke(sender, args);
            }
        }

    }

}

