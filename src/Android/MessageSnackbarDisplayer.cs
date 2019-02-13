using System;
using System.Collections.Generic;
using System.Linq;

using Android.Animation;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using SmartRoadSense.Shared;
using Android.Text;

namespace SmartRoadSense.Android {

    public class MessageSnackbarDisplayer : Java.Lang.Object, Animator.IAnimatorListener {

        private readonly Context _context;
        private readonly View _viewMessage;
        private readonly View _viewButton;

        private readonly ImageView _imageIcon;
        private readonly TextView _textTitle, _textDescription;

        private readonly float _buttonStopTranslationOn, _buttonStopTranslationOff, _buttonStopTranslationOffset;
        private readonly float _snackbarHeight;

        private Handler _handler;

        public const int LongDuration = 10 * 1000;
        public const int ShortDuration = 5 * 1000;
        public const int VeryShortDuration = 2 * 1000;

        public MessageSnackbarDisplayer(Context context, View viewMessage, View viewButton){
            _context = context;
            _viewMessage = viewMessage;
            _viewButton = viewButton;

            _viewMessage.Clickable = true;
            _viewMessage.Click += HandleSnackbarClick;

            _handler = new Handler(_context.MainLooper);

            _imageIcon = _viewMessage.FindViewById<ImageView>(Resource.Id.snackbar_icon);
            _textTitle = _viewMessage.FindViewById<TextView>(Resource.Id.snackbar_text);
            _textDescription = _viewMessage.FindViewById<TextView>(Resource.Id.snackbar_description);

            //Load values
            _buttonStopTranslationOn = context.Resources.GetDimension(Resource.Dimension.floating_button_on_screen_translation);
            _buttonStopTranslationOff = context.Resources.GetDimension(Resource.Dimension.floating_button_off_screen_translation);
            _buttonStopTranslationOffset = context.Resources.GetDimension(Resource.Dimension.floating_button_offset_translation);
            _snackbarHeight = context.Resources.GetDimension(Resource.Dimension.snackbar_height);

            //Init button UI
            if (_viewButton != null) {
                _viewButton.TranslationY = _buttonStopTranslationOff;
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

        Animator _animatorSlideOut;

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

            _imageIcon.SetImageDrawable(queueEntry.Message.GetIcon(_context));
            _textTitle.Text = queueEntry.Message.GetTitle(_context);
            _textTitle.SetTextColor(queueEntry.Message.GetTitleColor(_context));
            _textDescription.TextFormatted = Html.FromHtml(queueEntry.Message.GetDescription(_context));
            _viewMessage.SlideIn(_snackbarHeight, 1000, this);

            if(_isButtonShown && _viewButton != null) {
                PrepareButtonAnimation();
                _animatorButton = _viewButton.DecelerateToY(_buttonStopTranslationOffset, 1000);
            }

            //Setup timer for hiding if timed message
            if(!queueEntry.IsPersistent) {
                Log.Debug("Starting timer for message hiding in {0} ms", queueEntry.Duration);

                try {
                    var messageToHide = _currentMessage.Value;
                    _handler.PostDelayed(new Action(() => {
                        Log.Debug("Message hiding timer fired");
                        Hide(messageToHide);
                    }), queueEntry.Duration);
                }
                catch(ObjectDisposedException) {
                    // Noop: if the handler is disposed, the app is tearing down
                }
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

            if(_animatorSlideOut != null && _animatorSlideOut.IsRunning) {
                //Message to remove is still animating in, kill animation
                _animatorSlideOut.Cancel();
                _animatorSlideOut = null;
                _currentMessage = null;
            }

            Log.Debug("Hiding currently shown message {0}", _currentMessage);
            _animatorSlideOut = _viewMessage.SlideOut(500, this);

            if(_isButtonShown && _viewButton != null) {
                PrepareButtonAnimation();
                _viewButton.DecelerateToY(_buttonStopTranslationOn, 500);
            }
        }

        bool _isButtonShown = false;
        Animator _animatorButton = null;

        public void ShowButton(bool skipAnimations = false) {
            if(_isButtonShown || _viewButton == null)
                return;

            Log.Debug("Showing stop button");
            _isButtonShown = true;

            float targetMargin = _buttonStopTranslationOn;

            //Use offset margin if we have a message and it is not animating out
            if(_currentMessage != null && _animatorSlideOut == null) {
                targetMargin = _buttonStopTranslationOffset;
            }

            PrepareButtonAnimation();
            if (skipAnimations) {
                _viewButton.TranslationY = targetMargin;
            }
            else {
                _animatorButton = _viewButton.BounceToY(targetMargin, 1500);
            }
        }

        public void HideButton(bool skipAnimations = false) {
            if(!_isButtonShown || _viewButton == null)
                return;

            Log.Debug("Hiding stop button");
            _isButtonShown = false;

            PrepareButtonAnimation();
            if (skipAnimations) {
                _viewButton.TranslationY = _buttonStopTranslationOff;
            }
            else {
                _animatorButton = _viewButton.AnticipateToY(_buttonStopTranslationOff, 500);
            }
        }

        /// <summary>
        /// Prepares the button for an animation.
        /// </summary>
        private void PrepareButtonAnimation() {
            if(_animatorButton != null) {
                _animatorButton.Cancel();
                _animatorButton = null;
            }
        }

        #region

        public void OnAnimationCancel(Animator animation) {

        }

        public void OnAnimationEnd(Animator animation) {
            if(_animatorButton == animation) {
                _animatorButton = null;
            }
            else if(_animatorSlideOut == animation) {
                Log.Debug("Slide out animation completed for {0}", _currentMessage);

                _currentMessage = null;
                _animatorSlideOut = null;

                //Continue with next message if any
                if (_messages.Count > 0) {
                    InformationMessageQueueEntry entry = _messages.Dequeue();
                    Log.Debug("Processing information message {0} from queue", entry.Message);

                    InitShow(entry);
                }
            }
        }

        public void OnAnimationRepeat(Animator animation) {

        }

        public void OnAnimationStart(Animator animation) {

        }

        #endregion

        private void HandleSnackbarClick(object sender, EventArgs e) {
            if (!_currentMessage.HasValue)
                return;

            switch (_currentMessage.Value) {
                case InformationMessage.GpsDisabled:
                    _context.StartLocationSettings();
                    break;

                case InformationMessage.InternalEngineError:
                    _context.OpenErrorReporting();
                    break;
            }
        }

    }

}

