using System;
using Android.Views;
using Android.Widget;
using Android.Text;
using SmartRoadSense.Shared;

namespace SmartRoadSense.Android {

    public static class ViewExtensions {

        /// <summary>
        /// Reloads the text of the View as formatted HTML.
        /// </summary>
        public static void ReloadTextAsHtml(this TextView v) {
            var original = v.Text;
            v.TextFormatted = Html.FromHtml(original);
        }

		/// <summary>
		/// Returns visible for true, gone for false.
		/// </summary>
        public static ViewStates FalseToGone(this bool b) {
            if(b)
                return ViewStates.Visible;
            else
                return ViewStates.Gone;
        }

		/// <summary>
		/// Returns gone for true, visible for false.
		/// </summary>
		public static ViewStates TrueToGone(this bool b) {
			if(b)
				return ViewStates.Gone;
			else
				return ViewStates.Visible;
		}

        public static void SetConditionalColorFilter(this ImageView view, bool condition) {
            if (condition)
                view.SetColorFilter(App.Context.Resources.GetColor(Resource.Color.theme_primary));
            else
                view.ClearColorFilter();
        }

        public static void SetConditionalHighlight(this ImageView view, bool condition) {
            view.SetConditionalColorFilter(condition);

            if (condition)
                view.ScaleX = view.ScaleY = 1.2f;
            else
                view.ScaleX = view.ScaleY = 1f;
        }

    }

}
