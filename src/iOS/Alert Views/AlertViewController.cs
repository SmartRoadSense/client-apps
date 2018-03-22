
using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using SmartRoadSense.Shared;
using CoreFoundation;
using System.Threading;

namespace SmartRoadSense.iOS
{
	public partial class AlertViewController : UIViewController
	{
		public const int LongDuration = 10 * 1000;
		public const int ShortDuration = 5 * 1000;
		public const int VeryShortDuration = 2 * 1000;

		bool _isButtonShown = false;
		bool _isAnimating = false;
		bool _isButtonAnimating = false;

		private readonly float _buttonStopTranslationOn = -16;
		private readonly float _buttonStopTranslationOff = 56;
		private readonly float _buttonStopTranslationOffset = -118;
		private readonly float _snackbarHeight = 110;

		Action _slideOutAnimator;
		Action _buttonAnimator;
		UIButton _viewButton;

		//private Handler _handler;

		public AlertViewController (UIButton viewButton) : base ("AlertViewController", null)
		{
			this._viewButton = viewButton;
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
			lblBody.Text = "";
			lblTitle.Text = "";

			/*
			_viewMessage.Clickable = true;
			_viewMessage.Click += HandleSnackbarClick;

			*/

			//Init UI
			_viewButton.Transform.Translate (0, _buttonStopTranslationOff);
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
				Show(message);
			else
				Hide(message);
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

			if (_isAnimating) {
				//Message to remove is still animating in, kill animation
				_slideOutAnimator.EndInvoke (null);
				_slideOutAnimator = null;
				_currentMessage = null;
			}

			Log.Debug("Hiding currently shown message {0}", _currentMessage);

			_slideOutAnimator = () => {
				var frameCenter = this.View.Center;
				frameCenter.Y = this.View.Center.Y + _buttonStopTranslationOn;
				this.View.Center = frameCenter;
			};
			Animate (_slideOutAnimator, 0.5f);

			if (_isButtonShown) {
				PrepareButtonAnimation ();
				_buttonAnimator = () => {
					var frameCenter = this._viewButton.Center;
					frameCenter.Y = this._viewButton.Center.Y + _buttonStopTranslationOn;
					this._viewButton.Center = frameCenter;
				};
				Animate(_buttonAnimator, 0.5f);
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
				_viewButton.Transform.Translate (0,targetMargin);
			}
			else {
				Action _buttonAnimator = () => {
					var frameCenter = this._viewButton.Center;
					frameCenter.Y = this._viewButton.Center.Y + targetMargin;
					this._viewButton.Center = frameCenter;
				};
				Animate(_buttonAnimator, 1.5f);
			}

		}

		public void HideButton(bool skipAnimations = false) {
			if(!_isButtonShown)
				return;

			Log.Debug("Hiding stop button");
			_isButtonShown = false;

			PrepareButtonAnimation();

			if (skipAnimations) {
				_viewButton.Transform.Translate (0,_buttonStopTranslationOff);
			}
			else {
				_buttonAnimator = () => {
					var frameCenter = this._viewButton.Center;
					frameCenter.Y = this._viewButton.Center.Y + _buttonStopTranslationOff;
					this._viewButton.Center = frameCenter;
				};
				Animate(_buttonAnimator, 0.5f);
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
			if(_currentMessage == queueEntry.Message)
				return;
			foreach(var m in _messages) {
				if(m.Message == queueEntry.Message)
					return;
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
			imgIcon.Image = queueEntry.Message.GetIcon();
			lblTitle.Text = queueEntry.Message.GetTitle().PrepareForLabel ();
			lblTitle.TextColor = queueEntry.Message.GetTitleColor ();

			_slideOutAnimator = () => {
				var frameCenter = this.View.Center;
				frameCenter.Y = this.View.Center.Y + _snackbarHeight;
				this.View.Center = frameCenter;
			};
			Animate (_slideOutAnimator, 1.0f);

			if(_isButtonShown) {
				PrepareButtonAnimation();

				_buttonAnimator = () => {
					var frameCenter = this._viewButton.Center;
					frameCenter.Y = this._viewButton.Center.Y + _buttonStopTranslationOffset;
					this._viewButton.Center = frameCenter;
				};
				Animate (_buttonAnimator, 1.0f);
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

		private void HandleSnackbarClick(object sender, EventArgs e) {
			if (!_currentMessage.HasValue)
				return;

			switch (_currentMessage.Value) {
			case InformationMessage.GpsDisabled:
				//_context.StartLocationSettings();
				break;

			case InformationMessage.InternalEngineError:
				//_context.OpenErrorReporting();
				break;
			}
		}

		private void Animate (Action action, float time)
		{
			
			UICompletionHandler completion = new UICompletionHandler (delegate(bool finished) {
				if (!finished){
					Console.WriteLine ("animation didn't finish!");
				} else
					Console.WriteLine ("animation finished");
				//Continue with next message if any
				if (_messages.Count > 0) {
					InformationMessageQueueEntry entry = _messages.Dequeue();
					Log.Debug("Processing information message {0} from queue", entry.Message);

					InitShow(entry);
				}
			});

			InvokeOnMainThread (() => {
				UIView.AnimateNotify (
					time, // animation time
					0.0f, // animation delay
					0.3f, // spring dampening ration (lower == springyer)
					0.1f, // initial spring velocity
					UIViewAnimationOptions.AllowAnimatedContent,
					action,	
					null
				);
			});
		}
		/*

        Animator _slideOutAnimator;

        Animator _animatorButton = null;

        #region

        public void OnAnimationEnd(Animator animation) {
            if(_animatorButton == animation) {
                _animatorButton = null;
            }
            else if(_slideOutAnimator == animation) {
                Log.Debug("Slide out animation completed for {0}", _currentMessage);

                _currentMessage = null;
                _slideOutAnimator = null;

                //Continue with next message if any
                if (_messages.Count > 0) {
                    InformationMessageQueueEntry entry = _messages.Dequeue();
                    Log.Debug("Processing information message {0} from queue", entry.Message);

                    InitShow(entry);
                }
            }
        }
		*/
	}
}

