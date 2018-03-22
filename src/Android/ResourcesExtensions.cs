using System;
using SmartRoadSense.Shared;
using Android.Content;

namespace SmartRoadSense.Android {

    public static class ResourcesExtensions {

        public static int GetIconId(this VehicleType vehicle) {
            switch (vehicle) {
                case VehicleType.Car:
                default:
                    return Resource.Drawable.icon_car;

                case VehicleType.Motorcycle:
                    return Resource.Drawable.icon_motorcycle;

                case VehicleType.Truck:
                    return Resource.Drawable.icon_bus;
            }
        }

        public static int GetStringId(this VehicleType vehicle) {
            switch (vehicle) {
                case VehicleType.Car:
                default:
                    return Resource.String.Vernacular_P0_vehicle_car;

                case VehicleType.Motorcycle:
                    return Resource.String.Vernacular_P0_vehicle_motorcycle;

                case VehicleType.Truck:
                    return Resource.String.Vernacular_P0_vehicle_truck;
            }
        }

        public static int GetIconId(this AnchorageType anchorage) {
            switch (anchorage) {
                case AnchorageType.MobileBracket:
                default:
                    return Resource.Drawable.icon_bracket;

                case AnchorageType.MobileMat:
                    return Resource.Drawable.icon_mat;

                case AnchorageType.Pocket:
                    return Resource.Drawable.icon_pocket;
            }
        }

        public static int GetStringId(this AnchorageType anchorage) {
            switch (anchorage) {
                case AnchorageType.MobileBracket:
                default:
                    return Resource.String.Vernacular_P0_anchorage_bracket;

                case AnchorageType.MobileMat:
                    return Resource.String.Vernacular_P0_anchorage_mat;

                case AnchorageType.Pocket:
                    return Resource.String.Vernacular_P0_anchorage_pocket;
            }
        }

    }

}

