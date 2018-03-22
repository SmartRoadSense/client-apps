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

    public class Fragment4Anchorage : global::Android.Support.V4.App.Fragment, IDisplayAwareFragment {

        ImageView _imageMat, _imageBracket, _imagePocket;
        TextView _textSelected;

        bool _firstShow = true;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            var view = inflater.Inflate(Resource.Layout.fragment_tutorial_4_anchorage, container, false);

            _imageMat = view.FindViewById<ImageView>(Resource.Id.image_anchorage_mat);
            _imageMat.Click += (sender, e) => {
                Select(AnchorageType.MobileMat);
            };
            _imageBracket = view.FindViewById<ImageView>(Resource.Id.image_anchorage_bracket);
            _imageBracket.Click += (sender, e) => {
                Select(AnchorageType.MobileBracket);
            };
            _imagePocket = view.FindViewById<ImageView>(Resource.Id.image_anchorage_pocket);
            _imagePocket.Click += (sender, e) => {
                Select(AnchorageType.Pocket);
            };

            _textSelected = view.FindViewById<TextView>(Resource.Id.text_selected);

            //Reset animation
            if (_firstShow) {
                _imageMat.Visibility = _imageBracket.Visibility = _imagePocket.Visibility = ViewStates.Invisible;
                _textSelected.Visibility = ViewStates.Invisible;
            }
            else {
                Select(Settings.LastAnchorageType);
            }

			Log.Debug("View created");

            return view;
        }

		public override void OnDestroyView() {
			base.OnDestroyView();

			Log.Debug("View destroyed");
		}

        #region IDisplayAwareFragment implementation

        public void Shown() {
			Log.Debug("Shown (first shown {0}, view {1})", _firstShow, View);

			if(View == null)
				return;

            if (_firstShow) {
                _imageMat.FadeIn(1500, 0);
				_imageBracket.FadeIn(1500, 500);
				_imagePocket.FadeIn(1500, 1000);
				new Handler(Activity.MainLooper).PostDelayed(() => {
					Select(Settings.LastAnchorageType);
					_textSelected.Emerge(1000);
				}, 1750);

                _firstShow = false;
            }
        }

        public void Hidden() {
			Log.Debug("Hidden");
        }

        #endregion

        private void Select(AnchorageType anchorage) {
			if(Activity == null)
				return;
			
            _imageMat.SetConditionalHighlight(anchorage == AnchorageType.MobileMat);
            _imageBracket.SetConditionalHighlight(anchorage == AnchorageType.MobileBracket);
            _imagePocket.SetConditionalHighlight(anchorage == AnchorageType.Pocket);

            switch (anchorage) {
                default:
                case AnchorageType.MobileMat:
                    _textSelected.Text = GetString(Resource.String.Vernacular_P0_anchorage_mat);
                    break;

                case AnchorageType.MobileBracket:
                    _textSelected.Text = GetString(Resource.String.Vernacular_P0_anchorage_bracket);
                    break;

                case AnchorageType.Pocket:
                    _textSelected.Text = GetString(Resource.String.Vernacular_P0_anchorage_pocket);
                    break;
            }

            Settings.LastAnchorageType = anchorage;
        }

    }

}
