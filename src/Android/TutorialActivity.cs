using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

using SmartRoadSense.Android.Tutorial;
using SmartRoadSense.Shared;

namespace SmartRoadSense.Android {
    
    [Activity(
        Label = "@string/Vernacular_P0_title_tutorial"
    )]
    public class TutorialActivity : AppCompatActivity {

        private class TutorialFragmentPagerAdapter : FragmentPagerAdapter {

            public TutorialFragmentPagerAdapter(global::Android.Support.V4.App.FragmentManager manager)
                : base(manager) {

            }

            #region implemented abstract members of PagerAdapter

            public override int Count {
                get {
                    return 7;
                }
            }

            #endregion

            #region Implemented abstract members of FragmentPagerAdapter

            public override global::Android.Support.V4.App.Fragment GetItem(int position) {
                switch(position) {
                    case 0:
                        return new Fragment1Introduction();
                    case 1:
                        return new Fragment2What();
                    case 2:
                        return new Fragment3Vehicle();
                    case 3:
                        return new Fragment4Anchorage();
                    case 4:
                        return new Fragment5Calibration();
                    case 5:
                        return new Fragment6RideSharing();
                    case 6:
                        return new Fragment7Ready();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            #endregion

        }

        private ViewPager _pager;
        private TutorialFragmentPagerAdapter _pagerAdapter;
        private int _previousFragmentIndex = -1;
        private ImageView[] _pagerIndicators;

        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);

            //Translucent system bar
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
                Window.AddFlags(WindowManagerFlags.TranslucentStatus);
            }

            SetContentView(Resource.Layout.activity_tutorial);

            _pager = FindViewById<ViewPager>(Resource.Id.pager);
            _pager.Adapter = _pagerAdapter = new TutorialFragmentPagerAdapter(SupportFragmentManager);
            _pager.PageSelected += HandlePageSelected;

            _pagerIndicators = new ImageView[] {
                FindViewById<ImageView>(Resource.Id.pager_indicator_1),
                FindViewById<ImageView>(Resource.Id.pager_indicator_2),
                FindViewById<ImageView>(Resource.Id.pager_indicator_3),
                FindViewById<ImageView>(Resource.Id.pager_indicator_4),
                FindViewById<ImageView>(Resource.Id.pager_indicator_5),
                FindViewById<ImageView>(Resource.Id.pager_indicator_6),
                FindViewById<ImageView>(Resource.Id.pager_indicator_7)
            };
            for (int i = 0; i < _pagerIndicators.Length; ++i) {
                int index = i;
                _pagerIndicators[i].Clickable = true;
                _pagerIndicators[i].Click += (sender, e) => {
                    _pager.SetCurrentItem(index, true);
                };
            }
        }

        /// <summary>
        /// Gets the fragment at position.
        /// </summary>
        /// <remarks>
        /// http://stackoverflow.com/a/11976663/3118
        /// </remarks>
        private global::Android.Support.V4.App.Fragment GetFragmentAt(int position) {
            var tag = string.Format("android:switcher:{0}:{1}", _pager.Id, position);
            return SupportFragmentManager.FindFragmentByTag(tag);
        }

        private void HandlePageSelected(object sender, ViewPager.PageSelectedEventArgs e) {
            //Display awareness callbacks
            if (_previousFragmentIndex > 0 && _previousFragmentIndex < _pagerAdapter.Count) {
                var f = GetFragmentAt(_previousFragmentIndex) as IDisplayAwareFragment;
                if (f != null)
                    f.Hidden();
            }
            var newF = GetFragmentAt(e.Position) as IDisplayAwareFragment;
            if (newF != null)
                newF.Shown();

            //Update pager indicator
            for (int i = 0; i < _pagerIndicators.Length; ++i) {
                if (i == e.Position)
                    _pagerIndicators[i].SetImageResource(Resource.Drawable.ic_launcher);
                else
                    _pagerIndicators[i].SetImageResource(Resource.Drawable.ic_launcher_disabled);
            }

            Log.Debug("Page {0} selected", e.Position);
        }

        public override void OnBackPressed() {
            if (_pager.CurrentItem > 0) {
                _pager.SetCurrentItem(_pager.CurrentItem - 1, true);
                return;
            }

            //Do not forward back presses if tutorial is being completed the first time
            if (Settings.DidShowTutorial) {
                Finish();
                return;
            }
        }

    }
}

