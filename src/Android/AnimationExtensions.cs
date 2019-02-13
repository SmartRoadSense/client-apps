using System;
using System.Collections.Generic;

using Android.Animation;
using Android.Graphics;
using Android.Views;
using Android.Views.Animations;
using Java.Lang;

namespace SmartRoadSense.Android {

    public static class AnimationExtensions {

        /// <summary>
        /// Animates a view by changing its Y translation property.
        /// </summary>
        public static ObjectAnimator TranslateToY(this View v, float to, long duration,
            ITimeInterpolator interpolator, Animator.IAnimatorListener listener) {

            float from = v.TranslationY;

            //No special skipping

            var animator = ObjectAnimator.OfFloat(v, "translationY", from, to);
            animator.SetInterpolator(interpolator);
            animator.SetDuration(duration);
            if (listener != null) {
                animator.AddListener(listener);
            }
            animator.Start();

            return animator;
        }

        public static ObjectAnimator BounceToY(this View v, float to, long duration) {
            return TranslateToY(v, to, duration, new BounceInterpolator(), null);
        }

        public static ObjectAnimator AnticipateToY(this View v, float to, long duration) {
            return TranslateToY(v, to, duration, new AnticipateInterpolator(), null);
        }

        public static ObjectAnimator DecelerateToY(this View v, float to, long duration) {
            return TranslateToY(v, to, duration, new DecelerateInterpolator(), null);
        }

        public static void FadeIn(this View v, long duration, long delay = 0, Action endAction = null) {
            v.Alpha = 0f;
            v.Visibility = ViewStates.Visible;

            var anim = v.Animate()
                .Alpha(1f)
                .SetDuration(duration)
                .SetStartDelay(delay);

            if (endAction != null)
                anim.WithEndAction(new Runnable(endAction));

            anim.Start();
        }

        public static void FadeOut(this View v, long duration, long delay = 0, Action endAction = null) {
            v.Alpha = 1f;
            v.Visibility = ViewStates.Visible;

            var anim = v.Animate()
                .Alpha(0f)
                .SetDuration(duration)
                .SetStartDelay(delay);

            if (endAction != null)
                anim.WithEndAction(new Runnable(endAction));

            anim.Start();
        }

        public static void Emerge(this View v, long duration, long delay = 0) {
            v.Alpha = 0f;
            v.TranslationY = 20;
            v.Visibility = ViewStates.Visible;

            v.Animate()
                .Alpha(1f)
                .TranslationY(0)
                .SetInterpolator(new DecelerateInterpolator())
                .SetDuration(duration)
                .SetStartDelay(delay)
                .Start();
        }

        public static ObjectAnimator SlideIn(this View v, float viewHeight, long duration, Animator.IAnimatorListener listener) {
            return TranslateToY(v, -viewHeight, duration, new DecelerateInterpolator(), listener);
        }

        public static ObjectAnimator SlideOut(this View v, long duration, Animator.IAnimatorListener listener) {
            return TranslateToY(v, 0f, duration, new DecelerateInterpolator(), listener);
        }

    }

}

