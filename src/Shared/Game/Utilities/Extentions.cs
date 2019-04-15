using System;
namespace SmartRoadSense.Shared {
    public static class Extentions {

        public static float Multiply(float value) {
            if(value < 1) {
                value *= 10;
                value = Multiply(value);
            }
            return value;
        }
    }
}
