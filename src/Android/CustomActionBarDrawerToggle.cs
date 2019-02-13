using System;
using Android.App;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using SmartRoadSense.Shared;
using Android.Support.V7.Widget;

namespace SmartRoadSense.Android {

    /// <summary>
    /// Custom action bar drawer toggle that provides events for drawer motion.
    /// </summary>
    public class CustomActionBarDrawerToggle : ActionBarDrawerToggle {

        public CustomActionBarDrawerToggle(Activity activity,
            DrawerLayout drawerLayout, Toolbar toolbar, int openDrawerContentDescRes, int closeDrawerContentDescRes)
            : base(activity, drawerLayout, toolbar, openDrawerContentDescRes, closeDrawerContentDescRes) {

        }

        public override void OnDrawerOpened(View drawerView) {
            base.OnDrawerOpened(drawerView);

            DrawerOpened.Raise(this);
        }

        public event EventHandler DrawerOpened;

        public override void OnDrawerClosed(View drawerView) {
            base.OnDrawerClosed(drawerView);

            DrawerClosed.Raise(this);
        }

        public event EventHandler DrawerClosed;

    }

}

