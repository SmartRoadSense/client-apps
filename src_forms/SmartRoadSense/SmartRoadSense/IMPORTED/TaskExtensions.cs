using System;
using System.Threading.Tasks;

namespace SmartRoadSense {

    public static class TaskExtensions {

        public static void Forget(this Task t) {
        }

        public static void Forget<T>(this Task<T> t) {
        }

    }

}

