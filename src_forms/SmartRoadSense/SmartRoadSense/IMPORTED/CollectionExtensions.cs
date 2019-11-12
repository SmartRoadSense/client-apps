using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartRoadSense {

    public static class CollectionExtensions {

        /// <summary>
        /// Clears a collection and adds one single value.
        /// </summary>
        public static void Set<T>(this ICollection<T> collection, T value) {
            collection.Clear();
            collection.Add(value);
        }

        /// <summary>
        /// Determines is an enumerable is empty or not.
        /// </summary>
        /// <remarks>
        /// Null enumerations are considered to be empty as well.
        /// </remarks>
        public static bool IsEmpty<T>(this IEnumerable<T> collection) {
            if (collection == null)
                return true;

            if (collection is ICollection<T>) {
                return ((ICollection<T>)collection).Count == 0;
            }

            return !(collection.Any());
        }

    }

}
