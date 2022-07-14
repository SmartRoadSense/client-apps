using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using SmartRoadSense.Shared;

namespace SmartRoadSense.Android.Tutorial {

    public class Fragment3Vehicle : global::AndroidX.Fragment.App.Fragment, IDisplayAwareFragment {

		public Fragment3Vehicle() {
			Log.Debug("CREATION {0}", System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this));
		}

        ImageView _imageMotorcycle, _imageCar, _imageTruck;
        TextView _textSelected;

        bool _firstShow = true;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            var view = inflater.Inflate(Resource.Layout.fragment_tutorial_3_vehicle, container, false);

            _imageMotorcycle = view.FindViewById<ImageView>(Resource.Id.image_vehicle_motorcycle);
            _imageMotorcycle.Click += (sender, e) => {
                Select(VehicleType.Motorcycle);
            };
            _imageCar = view.FindViewById<ImageView>(Resource.Id.image_vehicle_car);
            _imageCar.Click += (sender, e) => {
                Select(VehicleType.Car);
            };
            _imageTruck = view.FindViewById<ImageView>(Resource.Id.image_vehicle_truck);
            _imageTruck.Click += (sender, e) => {
                Select(VehicleType.Truck);
            };

            _textSelected = view.FindViewById<TextView>(Resource.Id.text_selected);

            //Reset animation
            if (_firstShow) {
                _imageMotorcycle.Visibility = _imageCar.Visibility = _imageTruck.Visibility = ViewStates.Invisible;
                _textSelected.Visibility = ViewStates.Invisible;
            }
            else {
                Select(Settings.LastVehicleType);
            }

			Log.Debug("View created ({0})", System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this));

            return view;
        }

		public override void OnDestroyView() {
			base.OnDestroyView();

			Log.Debug("View destroyed ({0})", System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this));
		}

        #region IDisplayAwareFragment implementation

        public void Shown() {
			Log.Debug("Shown (first shown {0}, view {1}) ({2})", _firstShow, View, System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this));

			if(View == null)
				return;

            if (_firstShow) {
                _imageMotorcycle.FadeIn(1500, 0);
				_imageCar.FadeIn(1500, 500);
				_imageTruck.FadeIn(1500, 1000);
				new Handler(Activity.MainLooper).PostDelayed(() => {
					Select(Settings.LastVehicleType);
					_textSelected.Emerge(1000);
				}, 1750);

                _firstShow = false;
            }
        }

        public void Hidden() {
			Log.Debug("Hidden");
        }

        #endregion

        private void Select(VehicleType vehicle) {
			if(Activity == null)
				return;
			
            _imageMotorcycle.SetConditionalHighlight(vehicle == VehicleType.Motorcycle);
            _imageCar.SetConditionalHighlight(vehicle == VehicleType.Car);
            _imageTruck.SetConditionalHighlight(vehicle == VehicleType.Truck);

            switch (vehicle) {
                default:
                case VehicleType.Car:
                    _textSelected.Text = GetString(Resource.String.Vernacular_P0_vehicle_car);
                    break;

                case VehicleType.Motorcycle:
                    _textSelected.Text = GetString(Resource.String.Vernacular_P0_vehicle_motorcycle);
                    break;

                case VehicleType.Truck:
                    _textSelected.Text = GetString(Resource.String.Vernacular_P0_vehicle_truck);
                    break;
            }

            Settings.LastVehicleType = vehicle;
        }

    }

}
