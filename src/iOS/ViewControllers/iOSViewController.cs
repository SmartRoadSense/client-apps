using System;
using System.Drawing;

using Foundation;
using UIKit;

using System.Runtime.InteropServices;
using AudioToolbox;
using CoreAnimation;
using AVFoundation;
using SmartRoadSense.Shared;
using CoreText;
using SmartRoadSense.iOS;
using CoreTelephony;
using SmartRoadSense.Shared.ViewModel;
using SmartRoadSense.Shared.Data;
using SmartRoadSense.Core;
using SidebarNavigation;
using CoreGraphics;
using System.Net.NetworkInformation;
using iOS;
using System.Threading;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using ObjCRuntime;
using System.Net.Sockets;

namespace SmartRoadSense.iOS
{
	public partial class iOSViewController : UIViewController
	{
		CTCallCenter callcenter = new CTCallCenter ();
		CallStateSettings callState = new CallStateSettings ();

		// SRS Recording Engine
		RecordingViewModel RVM;

		UIBarButtonItem menuButton;
		UIBarButtonItem[] barButtonItems = new UIBarButtonItem[1];

		public const int LongDuration = 10 * 1000;
		public const int ShortDuration = 5 * 1000;
		public const int VeryShortDuration = 2 * 1000;

		bool _isButtonShown = false;
		bool _isButtonAnimating = false;
		bool _stopButtonVisible = false;

		private  float _buttonStopTranslationOn = -0;
		private  float _buttonStopTranslationOff = 0;
		private  float _buttonStopTranslationOffset = +0;

		private nfloat stopButtonTranslation = 0;
		private nfloat alertViewTopTranslation = 0;

		Action _buttonAnimator;
		//Action _slideOutAnimator;

		bool HeadphonesConnected = false;

		// provide access to the sidebar controller to all inheriting controllers
		protected SidebarNavigation.SidebarController SidebarController { 
			get {
				return (UIApplication.SharedApplication.Delegate as AppDelegate).RootViewController.SidebarController;
			} 
		}

		// provide access to the sidebar controller to all inheriting controllers
		protected NavController NavController { 
			get {
				return (UIApplication.SharedApplication.Delegate as AppDelegate).RootViewController.NavController;
			} 
		}

		public iOSViewController (IntPtr handle) : base (handle)
		{
			this.Title = NSBundle.MainBundle.LocalizedString ("Vernacular_P0_title_main", null);
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Initialize Recorder
			RVM = new RecordingViewModel();

			// Perform any additional setup after loading the view, typically from a nib.
			// Init audio session
			AudioSession.Initialize (null, NSRunLoop.NSDefaultRunLoopMode);
			AudioSession.AudioRouteChanged += AudioSession_AudioRouteChanged;

			// Init call event handler
			callcenter.CallEventHandler += callState.CallEvent;
					
			// Set menu button
			UIImageView menuImageView = new UIImageView ();
			menuImageView.Bounds = new CGRect (0, 0, 20, 20);
			menuImageView.Image = UIImage.FromBundle ("threelines");

			menuButton = new UIBarButtonItem (
				menuImageView.Image,
				UIBarButtonItemStyle.Plain,
				(s, e) => {
					System.Diagnostics.Debug.WriteLine ("menu button tapped");
					SidebarController.ToggleMenu ();
				}
			);

			// Add button to item array
			barButtonItems [0] = menuButton;
			NavigationItem.LeftBarButtonItem = menuButton;

			// Set bottom view labels
			lblTitle.Text = "";
			lblBody.Text = "";

			// Set start button style
			btnStart.SetBackgroundImage (UIImage.FromBundle ("CarButton"), UIControlState.Normal);
			View.BackgroundColor = StyleSettings.ThemePrimaryDarkLightenedColor ();
			btnStart.SetTitle ("", UIControlState.Normal);

			// Add square to stop || TODO: change to image
			UIView stopSquare = new UIView (new RectangleF (
				                    (float)(btnStop.Bounds.X + 15), 
				                    (float)(btnStop.Bounds.Y + 15), 
				                    (float)(btnStop.Bounds.Width - 30), 
				                    (float)(btnStop.Bounds.Height - 30))
			                    );

			// Set stop button attributes
			stopButtonTranslation = btnStopBottomConstraint.Constant;
			stopSquare.BackgroundColor = UIColor.White;
			btnStop.SetBackgroundImage (UIImage.FromBundle ("srs-stop-btn"), UIControlState.Normal);
			btnStop.Layer.CornerRadius = btnStop.Bounds.Width / 2;

			lblLeft.Hidden = true;
			lblCenter.Hidden = true;
			lblRight.Hidden = true;

			// Set initial status of vehicle type & support
			btnSupport.Hidden = true;
			btnVehicle.Hidden = true;
			lblCalibration.Hidden = true;
			lblCalibration.Text = NSBundle.MainBundle.LocalizedString ("Vernacular_P0_label_setup", null);
			lblCalibration.TextColor = StyleSettings.SubduedTextOnDarkColor ();

			// Logic
			btnStart.TouchUpInside += (object sender, EventArgs e) => {
                if (Settings.CalibrationDone)
                {
                    if (RVM.IsRecording)
                        return;
                    
                    var passengerNumberPickerVC = Storyboard.InstantiateViewController("PassengerNumberPickerViewController") as PassengerNumberPickerViewController;
                    passengerNumberPickerVC.ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
					passengerNumberPickerVC.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                    passengerNumberPickerVC.parentVC = this;
					NavigationController.PresentViewController(passengerNumberPickerVC, true, null);
                } 
                else 
                {
                    // TODO: warn user that calibration has to be done
                }
			};

			btnStop.TouchUpInside += (object sender, EventArgs e) => {
				UnbindFromService ();
				AnimateStopButton ();
				RVM.OnDestroy ();
				StopRecording ();
				UpdateRecordButtonUi();
				UploadData ();
				lblCenter.Hidden = true;
			};

			btnVehicle.TouchUpInside += (object sender, EventArgs e) => {
				OpenSettingsVC ();
			};

			btnSupport.TouchUpInside += (object sender, EventArgs e) => {
				OpenSettingsVC ();
			};

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// Set vehicle and support buttons on every VC appearence
			btnVehicle = SetVehicleButton (btnVehicle);
			btnSupport = SetSupportButton (btnSupport);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			btnStop.Layer.CornerRadius = btnStop.Bounds.Width / 2;

			// Initialize constraint translation initial position
			btnStopBottomConstraint.Constant = stopButtonTranslation;
			alertViewTopTranslation = alertViewTopConstraint.Constant;
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}

        #endregion

        #region Start Recording

        public void InitRecording(){
			RVM.OnCreate();
			UnbindFromService();
			BindToService();
			if (!RVM.IsRecording)
				AnimateStopButton();

			if (Settings.LastNumberOfPeople == 1)
            {
                var carpoolingVC = Storyboard.InstantiateViewController("CarpoolingViewController") as CarpoolingViewController;
                carpoolingVC.ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
                carpoolingVC.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                carpoolingVC.parentVC = this;
                NavigationController.PresentViewController(carpoolingVC, true, null);
            }
			else
			{
                Log.Debug("no carpooling :( ");
                StartRecordingCommands();
			}			
        }

        public void StartRecordingCommands()
        {
			StartRecording();
			lblCenter.Hidden = false;  
        }

        #endregion

        #region Service binding

        private void BindToService() {
			if (RVM != null) {
				RVM.MeasurementsUpdated += HandleMeasurementsUpdated;
				RVM.RecordingStatusUpdated += HandleRecordingStatusUpdated;
				RVM.SensorStatusUpdated += HandleSensorStatusUpdated;
				RVM.RecordingSuspended += HandleRecordingSuspended;
				RVM.InternalEngineErrorReported += HandleInternalEngineError;
				RVM.SyncErrorReported += HandleSyncError;
			}
			else {
				Log.Debug("Unable to bind to sensing service model");
			}
		}

		private void UnbindFromService() {
			if (RVM != null) {
				RVM.MeasurementsUpdated -= HandleMeasurementsUpdated;
				RVM.RecordingStatusUpdated -= HandleRecordingStatusUpdated;
				RVM.SensorStatusUpdated -= HandleSensorStatusUpdated;
				RVM.RecordingSuspended -= HandleRecordingSuspended;
				RVM.InternalEngineErrorReported -= HandleInternalEngineError;
                RVM.SyncErrorReported -= HandleSyncError;
			}
			else {
				Log.Debug("Unable to unbind from sensing service model (no model)");
			}
		}

		#endregion

		#region handle model changes

		private const string MeasurementFormat = "{0:F1}";
		private const string MeasurementMinMaxFormat = "{{ {0:F1} }}";
		private const string MeasurementUnknown = "-.-";

		void HandleMeasurementsUpdated(object sender, EventArgs e) {
			if (RVM != null) {
				UpdateMeasurementsDisplay ();
			} else {
				Log.Debug ("RVM is null");
			}
		}

		void HandleRecordingStatusUpdated(object sender, EventArgs e) {
			if (RVM != null) {
				UpdateRecordButtonUi();

				if (RVM.IsRecording) {
					this.ShowButton();

					//Remove internal error message when switching to recording status
					this.Hide(InformationMessage.InternalEngineError);
				}
				else {
					this.HideButton();
				}
			}
		}

		void HandleSensorStatusUpdated(object sender, EventArgs e) {
			if (RVM != null) {
				UpdateMeasurementsDisplay();

				UpdateRecordButtonUi();

				UpdateBottomBarUi();
			} else
				Log.Debug ("Error handling updated sensor status");

		}

		private void HandleRecordingSuspendedSpeed(object sender, EventArgs e){
			this.Show(InformationMessage.GpsSuspendedSpeed, AlertViewController.LongDuration);
		}

		private void HandleInternalEngineError(object sender, EventArgs e) {
			this.Show(InformationMessage.InternalEngineError, AlertViewController.LongDuration);
		}

		private void HandleRecordingSuspended(object sender, LocationErrorEventArgs e)
		{
			switch (e.Error)
			{
				case LocationErrorType.RemainedStationary:
					this.Show(InformationMessage.GpsSuspendedStationary, AlertViewController.LongDuration);
					break;
				case LocationErrorType.SpeedTooLow:
					this.Show(InformationMessage.GpsSuspendedSpeed, AlertViewController.LongDuration);
					break;
			}
		}

		private void HandleSyncError(object sender, SyncErrorEventArgs e)
		{
            InvokeOnMainThread(() => {
				this.Show(InformationMessage.UploadFailure, AlertViewController.LongDuration);
			});
		}

		private void UpdateRecordButtonUi() 
        {
			if (RVM != null) {
				if (RVM.IsRecording)
				{
					if (RVM.LocationSensorStatus.IsActive())
                        InvokeOnMainThread(() => {
                        	btnStart.SetBackgroundImage(UIImage.FromBundle("button_car_on"), UIControlState.Normal);
                        });
					else
                        InvokeOnMainThread(() => {
                        	btnStart.SetBackgroundImage(UIImage.FromBundle("button_car_unknown"), UIControlState.Normal);
                        });
				}
				else
                    InvokeOnMainThread(() => {
                    	btnStart.SetBackgroundImage(UIImage.FromBundle("button_car"), UIControlState.Normal);
                    });
			}
			else
				InvokeOnMainThread(() => {
					btnStart.SetBackgroundImage(UIImage.FromBundle("button_car"), UIControlState.Normal);
				});
		}

		/// <summary>
		/// Updates the bottom bar UI based on the current status.
		/// Assumes the ViewModel is available.
		/// </summary>
		private void UpdateBottomBarUi() {
			//GPS status information bar
			if (RVM.IsRecording) {
				var sensorStatus = RVM.LocationSensorStatus;
				this.ShowIf (InformationMessage.GpsDisabled, sensorStatus == LocationSensorStatus.Disabled);
				this.Hide (InformationMessage.GpsSuspendedStationary);
				this.Hide (InformationMessage.GpsSuspendedSpeed);
				this.ShowIf (InformationMessage.GpsUnfixed, sensorStatus == LocationSensorStatus.Fixing);
				//TODO: out of country
			} else
				Log.Debug ("RVM is not recording");
		}
			
		/// <summary>
		/// Instantaneously updates the measurements display.
		/// Assumes the ViewModel is available.
		/// </summary>
		private void UpdateMeasurementsDisplay() {
			if (RVM.IsReporting) {
				lblCenter.Text = string.Format (MeasurementFormat, RVM.CurrentPpe);
				double range = RVM.MaximumPpe - RVM.MinimumPpe;
				double perc = (RVM.CurrentPpe - RVM.MinimumPpe) / range;
				lblCenter.TextColor = StyleSettings.InterpolateTextColor (StyleSettings.QualityGoodColor (), StyleSettings.QualityBadColor (), (float)perc);
			}
			else {
				Log.Debug ("RVM is not reporting");
				lblCenter.Text = NSBundle.MainBundle.LocalizedString ("Vernacular_P0_unknown_ppe_value", null);
				lblCenter.TextColor = StyleSettings.TextOnBrightColor ();
			}
		}
			
		#endregion

		#region animation/init-stop recordings

		void AnimateStopButton ()
		{
			UICompletionHandler completion = new UICompletionHandler (delegate(bool finished) {
				if (!finished){
					Log.Debug ("stop animation didn't finish!");
				} else
					Log.Debug ("stop animation finished");			

			});

			Action action = () => {
				if (!_stopButtonVisible) {
					btnStopBottomConstraint.Constant = bottomView.Bounds.Y + bottomView.Bounds.Height + btnStop.Bounds.Height/3;
					stopButtonTranslation = btnStopBottomConstraint.Constant;
				} else {
					btnStopBottomConstraint.Constant = 0 - btnStop.Frame.Height;
					stopButtonTranslation = btnStopBottomConstraint.Constant;
				}
				btnStop.NeedsUpdateConstraints ();
				_stopButtonVisible = !_stopButtonVisible;
				btnStop.LayoutIfNeeded ();
			};

			float springValue = 1.0f;
			float animationTime = 1.0f;
			if (!_stopButtonVisible) {
				springValue = 0.3f;
				animationTime = 1.8f;
			} else {
				springValue = 1.0f;
				animationTime = 0.8f;
			}

			InvokeOnMainThread (() => {
				UIView.AnimateNotify (
					animationTime, // animation time
					0.0f, // animation delay
					springValue, // spring dampening ration (lower == springyer)
					0.1f, // initial spring velocity
					UIViewAnimationOptions.CurveEaseIn,
					action,
					completion
				);
			});

		}

		private void Animation (float time, bool direction) {

			UICompletionHandler completion = new UICompletionHandler (delegate(bool finished) {
				if (!finished){
					Log.Debug ("bottom view animation didn't finish!");
				} else
					Log.Debug ("bottom view animation finished");
			});

			Action action = () => {
				if (direction) {
					alertViewTopConstraint.Constant = alertViewTopTranslation - bottomView.Frame.Height;
				} else {
					alertViewTopConstraint.Constant = alertViewTopTranslation;
				}
				bottomView.NeedsUpdateConstraints ();
				bottomView.LayoutIfNeeded ();
			};
				
			InvokeOnMainThread (() => {
				UIView.AnimateNotify (
					time, // animation time
					0.0f, // animation delay
					1.0f, // spring dampening ration (lower == springyer)
					0.1f, // initial spring velocity
					UIViewAnimationOptions.CurveLinear,
					action,
					completion
				);
			});
				
		}

		private void TraslateStopButton (Action action, float time) {
			
			InvokeOnMainThread (() => {
				UIView.AnimateNotify (
					time, // animation time
					0.0f, // animation delay
					1.0f, // spring dampening ration (lower == springyer)
					0.1f, // initial spring velocity
					UIViewAnimationOptions.CurveLinear,
					action,
					null
				);
			});
		}
				
		#endregion
	
		#region Bottom View Settings

		InformationMessage? _currentMessage = null;

		private class InformationMessageQueueEntry {

			public InformationMessageQueueEntry(InformationMessage message, long duration) {
				Message = message;
				Duration = duration;
			}

			public InformationMessage Message;

			public long Duration;

			public bool IsPersistent {
				get {
					return Duration < 0;
				}
			}
		}

		Queue<InformationMessageQueueEntry> _messages = new Queue<InformationMessageQueueEntry>();

		/// <summary>
		/// Shows a given message for a limited amount of time.
		/// </summary>
		/// <param name="duration">
		/// Duration in milliseconds.
		/// Message is persistent if duration is negative.
		/// </param>
		public void Show(InformationMessage message, long duration) {
			InitShow(new InformationMessageQueueEntry(message, duration));
		}

		private void InitShow(InformationMessageQueueEntry queueEntry) {
			//Drop if already existing messages
			if (_currentMessage == queueEntry.Message) {
				Log.Debug ("message \"{0}\" already displayed", queueEntry.Message);
				return;
			}foreach(var m in _messages) {
				if (m.Message == queueEntry.Message) {
					Log.Debug ("message \"{0}\" already in queue", queueEntry.Message);
					return;
				}
			}

			//If already showing a message, put new one in queue
			if(_currentMessage.HasValue) {
				Log.Debug("Message {0} queued because {1} is shown ({2} already queued)", queueEntry.Message, _currentMessage, _messages.Count);
				_messages.Enqueue(queueEntry);
				return;
			}

			//Here we go
			_currentMessage = queueEntry.Message;
			Log.Debug("Showing message {0}", _currentMessage);
			imgError.Image = queueEntry.Message.GetIcon();
			lblTitle.Text = queueEntry.Message.GetTitle().PrepareForLabel ();
			lblBody.Text = queueEntry.Message.GetDescription ().PrepareForLabel ();
			lblTitle.TextColor = queueEntry.Message.GetTitleColor ();

			Animation (1.0f, true);

			// remove stop button if too slow message or stopped moving message
			if (_currentMessage == InformationMessage.GpsSuspendedSpeed || _currentMessage == InformationMessage.GpsSuspendedStationary) {
				AnimateStopButton ();
				lblCenter.Hidden = true;
			}

			if(_isButtonShown) {
				PrepareButtonAnimation();

				_buttonAnimator = () => {
					btnStopBottomConstraint.Constant = btnStopBottomConstraint.Constant + _buttonStopTranslationOffset;
					btnStop.NeedsUpdateConstraints ();
					btnStop.LayoutIfNeeded ();
				};
				TraslateStopButton (_buttonAnimator, 1.0f);
			}

			//Setup timer for hiding if timed message
			if(!queueEntry.IsPersistent) {
				Log.Debug("Starting timer for message hiding in {0} ms", queueEntry.Duration);

				var messageToHide = _currentMessage.Value;
				InvokeInBackground (new Action (() => {
					Thread.Sleep((int)queueEntry.Duration);
					Log.Debug ("Message hiding timer fired");
					Hide (messageToHide);
				}));

			}

		}

		/// <summary>
		/// Shows a given message until removed.
		/// </summary>
		public void Show(InformationMessage message) {
			InitShow(new InformationMessageQueueEntry(message, -1));
		}

		/// <summary>
		/// Shows a given message if a condition is satisfied. Otherwise the message is removed.
		/// </summary>
		public void ShowIf(InformationMessage message, bool condition) {
			if (condition)
				Show (message);
			else {
				Hide(message);
			}
		}

		public void Hide(InformationMessage message) {
			//Remove pending messages of same type
			_messages = new Queue<InformationMessageQueueEntry>(from m in _messages
				where m.Message != message
				select m);

			if(_currentMessage != message) {
				Log.Debug("Cannot remove message {0} since it is not shown currently", message);
				return;
			}
				
			if (bottomView.Layer.AnimationKeys != null) {
				//Message to remove is still animating in, kill animation
				bottomView.Layer.RemoveAllAnimations ();
				_currentMessage = null;
				Log.Debug ("view was animating");
			} else
				_currentMessage = null;

			Log.Debug("Hiding currently shown message {0}", _currentMessage);
			Animation (0.5f, false);

			if (_isButtonShown) {
				PrepareButtonAnimation ();
				_buttonAnimator = () => {
					btnStopBottomConstraint.Constant = btnStopBottomConstraint.Constant + _buttonStopTranslationOn;
					btnStop.NeedsUpdateConstraints ();
					btnStop.LayoutIfNeeded ();
				};
				TraslateStopButton (_buttonAnimator, 0.5f);
			}
		}

		public void ShowButton(bool skipAnimations = false) {
			if(_isButtonShown)
				return;

			Log.Debug("Showing stop button");
			_isButtonShown = true;

			float targetMargin = _buttonStopTranslationOn;
			if(_currentMessage != null) {
				targetMargin = _buttonStopTranslationOffset;
			}

			PrepareButtonAnimation();

			if (skipAnimations) {
				btnStop.Transform.Translate (0, targetMargin);
			}
			else {
				_buttonAnimator = () => {
					btnStopBottomConstraint.Constant = btnStopBottomConstraint.Constant + targetMargin;
					btnStop.NeedsUpdateConstraints ();
					btnStop.LayoutIfNeeded ();
				};
				TraslateStopButton (_buttonAnimator, 0.5f);
			}
		}

		public void HideButton(bool skipAnimations = false) {
			if(!_isButtonShown)
				return;

			Log.Debug("Hiding stop button");
			_isButtonShown = false;

			PrepareButtonAnimation();


			if (skipAnimations) {
				btnStop.Transform.Translate (0, _buttonStopTranslationOff);
			}
			else {
				_buttonAnimator = () => {
					btnStopBottomConstraint.Constant = btnStopBottomConstraint.Constant + _buttonStopTranslationOff;
					btnStop.NeedsUpdateConstraints ();
					btnStop.LayoutIfNeeded ();
				};
				TraslateStopButton (_buttonAnimator, 0.5f);
			}


		}

		/// <summary>
		/// Prepares the button for an animation.
		/// </summary>
		private void PrepareButtonAnimation() {
			
			if(_isButtonAnimating) {
				_buttonAnimator.EndInvoke (null);
				_buttonAnimator = null;
			}

		}

		#endregion
	
		#region audio events

		void AudioSession_AudioRouteChanged (object sender, AudioSessionRouteChangeEventArgs e)
		{
			if (e.CurrentOutputRoutes [0] == AudioSessionOutputRouteKind.Headphones) {
				//Code when is plugged
				HeadphonesConnected = true;
			} else {
				//Code when is unplugged  
				HeadphonesConnected = false;
			}
		}

		private void CallEvent (CTCall inCTCall)
		{
			if (!HeadphonesConnected && RVM.IsRecording) {
				if(inCTCall.CallState == "CTCallStateDialing" ||
					inCTCall.CallState == "CTCallStateIncoming" ||
					inCTCall.CallState == "CTCallStateConnected")
					RVM.StopRecordingCommand.Execute (null);

				else if(inCTCall.CallState == "CTCallStateDisconnected")
					RVM.StartRecordingCommand.Execute (null);
			}
		}

		#endregion

		#region data upload

		private async void UploadData(){

			SyncManager SyncManager = new SyncManager();
			if (SyncManager.CheckSyncConditions()) {
				if ((Reachability.InternetConnectionStatus () == NetworkStatus.ReachableViaWiFiNetwork) ||
					(Reachability.InternetConnectionStatus () != NetworkStatus.ReachableViaWiFiNetwork && !Settings.PreferUnmeteredConnection)) {
					var src = new CancellationTokenSource ();
					var token = src.Token;
					await SyncManager.Synchronize (token);
				} 
			}
		}

		#endregion

		#region vehicle type and support type

		private UIButton SetVehicleButton(UIButton btn){

			// Set button backgrounds
			btn.ClipsToBounds = true;
			btn.ContentMode = UIViewContentMode.ScaleAspectFit;

			// Vehicle type
			if (Settings.LastVehicleType == VehicleType.Motorcycle) {
				btn.SetBackgroundImage (ChangeImageColor.GetColoredImage ("icon_motorcycle", StyleSettings.ThemePrimaryColor ()), UIControlState.Normal);
			} else if (Settings.LastVehicleType == VehicleType.Car ) {
				btn.SetBackgroundImage (ChangeImageColor.GetColoredImage ("icon_car", StyleSettings.ThemePrimaryColor ()), UIControlState.Normal);
			} else if (Settings.LastVehicleType == VehicleType.Truck) {
				btn.SetBackgroundImage (ChangeImageColor.GetColoredImage ("icon_bus", StyleSettings.ThemePrimaryColor ()), UIControlState.Normal);
			}

			return btn;
		}

		private UIButton SetSupportButton(UIButton btn) {

			btn.ClipsToBounds = true;
			btn.ContentMode = UIViewContentMode.ScaleAspectFit;

			// Anchorage type
			if (Settings.LastAnchorageType == AnchorageType.MobileBracket) {
				btn.SetBackgroundImage (ChangeImageColor.GetColoredImage ("icon_bracket", StyleSettings.ThemePrimaryColor ()), UIControlState.Normal);
			} else if (Settings.LastAnchorageType == AnchorageType.MobileMat) {
				btn.SetBackgroundImage (ChangeImageColor.GetColoredImage ("icon_mat", StyleSettings.ThemePrimaryColor ()), UIControlState.Normal);
			} else if (Settings.LastAnchorageType == AnchorageType.Pocket) {
				btn.SetBackgroundImage (ChangeImageColor.GetColoredImage ("icon_pocket", StyleSettings.ThemePrimaryColor ()), UIControlState.Normal);
			}			
			return btn;
		}

		public void OpenSettingsVC() {
			var storyboard = UIStoryboard.FromName ("MainStoryboard", null);
			var SettingsVC = storyboard.InstantiateViewController ("SettingsViewController") as SettingsViewController;
			NavController.PushViewController (SettingsVC, true);
			SidebarController.CloseMenu (true);
		}

		#endregion

		#region start/stop engine

		private void StartRecording(){
			if (RVM != null) {
				RVM.StartRecordingCommand.Execute(null);
				btnSupport.Hidden = false;
				btnVehicle.Hidden = false;
				lblCalibration.Hidden = false;
			}
			else {
				Log.Warning(new NullReferenceException(), "Failed to start recording: no model");
			}
		}

		private void StopRecording(){
			if (RVM != null) {
				RVM.StopRecordingCommand.Execute(null);
				btnSupport.Hidden = true;
				btnVehicle.Hidden = true;
				lblCalibration.Hidden = true;
			}
			else {
				Log.Warning(new NullReferenceException(), "Failed to stop recording: no model");
			}
		}

		#endregion
	}
}

