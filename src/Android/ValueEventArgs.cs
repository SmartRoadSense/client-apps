using System;

namespace SmartRoadSense.Android {

    public class ValueEventArgs<T> : EventArgs {

        private readonly T _value;

        public ValueEventArgs(T v) {
            _value = v;
        }

        public T Value {
            get {
                return _value;
            }
        }

    }

}