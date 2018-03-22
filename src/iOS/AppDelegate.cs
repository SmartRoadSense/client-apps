using System;
using Foundation;
using UIKit;
using System.Threading;
using SmartRoadSense.Shared;

namespace SmartRoadSense.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {

        // class-level declarations
        public override UIWindow Window
        {
            get;
            set;
        }

        public RootViewController RootViewController { get { return Window.RootViewController as RootViewController; } }

        // This method is invoked when the application is about to move from active to inactive state.
        // OpenGL applications should use this method to pause.
        public override void OnResignActivation(UIApplication application)
        {
            App.Suspended().Wait();
        }

        // This method should be used to release shared resources and it should store the application state.
        // If your application supports background exection this method is called instead of WillTerminate
        // when the user quits.
        public override void DidEnterBackground(UIApplication application)
        {
            App.Suspended().Wait();
        }

        // This method is called as part of the transiton from background to active state.
        public override void WillEnterForeground(UIApplication application)
        {
            App.Activated();
        }

        // This method is called when the application is about to terminate. Save data, if needed.
        public override void WillTerminate(UIApplication application)
        {
            App.Suspended().Wait();
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // create a new window instance based on the screen size
            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            // Initialize App
            App.Initialize().Wait();
            App.Activated();

            // Check if tutorial has been seen
            if (NSUserDefaults.StandardUserDefaults.BoolForKey(PreferencesSettings.FirstLaunchKey))
            {
                // If you have defined a root view controller, set it here:
                Window.RootViewController = new RootViewController();
            }
            else
            {
                UIStoryboard storyboard = UIStoryboard.FromName("MainStoryboard", null);
                IntroductionPageViewController introductionVC = storyboard.InstantiateViewController("IntroductionPageViewController") as IntroductionPageViewController;
                Window.RootViewController = introductionVC;
                UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.BlackTranslucent, false);
                UIApplication.SharedApplication.SetStatusBarHidden(false, false);
            }

            // Set background fetch interval
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);

            // make the window visible
            Window.MakeKeyAndVisible();

            return true;
        }

        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            // Check for new data, and display it
            UploadData();
            Log.Debug("trying to upload SRS data");

            // Inform system of fetch results
            completionHandler(UIBackgroundFetchResult.NewData);
        }

        public async void UploadData()
        {
            SyncManager SyncManager = new SyncManager();
            if (SyncManager.CheckSyncConditions())
            {
                Log.Debug("SRS background data upload");
                if ((Reachability.InternetConnectionStatus() == NetworkStatus.ReachableViaWiFiNetwork) ||
                    (Reachability.InternetConnectionStatus() != NetworkStatus.ReachableViaWiFiNetwork && !Settings.PreferUnmeteredConnection))
                {
                    var src = new CancellationTokenSource();
                    var token = src.Token;
                    await SyncManager.Synchronize(token);
                }
            }
            else
            {
                Log.Debug("SyncManager can't sync");
            }
        }

    }

}
