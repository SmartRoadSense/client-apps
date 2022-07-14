using System;
using Android.App;
using AndroidX.AppCompat.App;
using Android.Views;
using SmartRoadSense.Shared;
using AndroidX.DrawerLayout.Widget;
using AndroidX.AppCompat.Widget;

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

