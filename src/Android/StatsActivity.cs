using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;

using SmartRoadSense.Shared;
using SmartRoadSense.Shared.Database;
using Android.Text.Style;
using System.Threading.Tasks;
using Android.Graphics;

namespace SmartRoadSense.Android {

    [Activity(
        Label = "@string/Vernacular_P0_title_stats",
        ParentActivity = typeof(MainActivity)
    )]
    public class StatsActivity : AppCompatActivity {

        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.activity_stats);

            //Toolbar support
            var toolbar = this.FindViewById<global::Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if (toolbar != null) {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
                FindViewById(Resource.Id.toolbar_shadow).Visibility = ViewStates.Gone;
            }

            try {
                using (var conn = DatabaseUtility.OpenConnection()) {
                    var week = StatisticHelper.GetPeriodSummary(conn, StatisticPeriod.Week);
                    var overall = StatisticHelper.GetPeriodSummary(conn, StatisticPeriod.Overall);
                    var last = StatisticHelper.GetLastTrack(conn);

                    UpdateKmCounter(FindViewById<TextView>(Resource.Id.label_last_track_kms), last?.DistanceTraveled);
                    UpdateKmCounter(FindViewById<TextView>(Resource.Id.label_last_week_kms), week.Distance);
                    UpdateKmCounter(FindViewById<TextView>(Resource.Id.label_overall_kms), overall.Distance);
                }
            }
            catch(Exception ex) {
                Log.Error(ex, "Failed to load statistics");
            }

            FindViewById<Button>(Resource.Id.button_share).Click += HandleShareClick;
        }

        private void HandleShareClick(object sender, EventArgs e) {
            Log.Debug("Preparing sharable image");
            PrepareImageAndShare().Forget();
        }

        private void UpdateKmCounter(TextView tv, double? kms) {
            var integer = (kms.HasValue) ? kms.Value.ToString("0.") : "0";
            string formatted = GetString(Resource.String.Vernacular_P0_stats_kms_value_default);
            if(kms.HasValue)
                formatted = string.Format(GetString(Resource.String.Vernacular_P0_stats_kms_value_format), kms);

            var spannable = new SpannableString(formatted);
            spannable.SetSpan(new TextAppearanceSpan(ApplicationContext, Resource.Style.text_counter_number), 0, integer.Length, SpanTypes.ExclusiveExclusive);
            spannable.SetSpan(new TextAppearanceSpan(ApplicationContext, Resource.Style.text_counter_decimal), integer.Length, formatted.Length, SpanTypes.ExclusiveExclusive);
            tv.SetText(spannable, TextView.BufferType.Spannable);
        }

        public override bool OnOptionsItemSelected(IMenuItem item) {
            if (item == null)
                return false;

            switch (item.ItemId) {
                case global::Android.Resource.Id.Home:
                    NavUtils.NavigateUpFromSameTask(this);

                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private async Task PrepareImageAndShare() {
            Bitmap source = null, output = null;
            Canvas canvas = null;

            try {
                double kms = await Task<double>.Run(() => {
                    using (var conn = DatabaseUtility.OpenConnection()) {
                        var overall = StatisticHelper.GetPeriodSummary(conn, StatisticPeriod.Overall);
                        return overall.Distance;
                    }
                });

                var formatted = kms.ToString("0.0");
                Log.Debug("Preparing sharable badge for '{0}'", formatted);

                source = await BitmapFactory.DecodeResourceAsync(Resources, Resource.Drawable.award_laurel_template);
                output = Bitmap.CreateBitmap(source.Width, source.Height, Bitmap.Config.Argb8888);

                canvas = new Canvas(output);
                canvas.DrawBitmap(source, 0f, 0f, null);

                Paint paintText = new Paint(PaintFlags.AntiAlias);
                paintText.SetTypeface(Typeface.DefaultBold);
                paintText.TextSize = source.Width / (float)formatted.Length; // TODO: correctly compute text size to fill area
                paintText.Color = Color.White;
                paintText.SetStyle(Paint.Style.Fill);

                // Text bounds computation taken from https://chris.banes.me/2014/03/27/measuring-text/
                Rect outTextBounds = new Rect();
                paintText.GetTextBounds(formatted, 0, formatted.Length, outTextBounds);
                var textWidth = paintText.MeasureText(formatted);
                var textHeight = (float)outTextBounds.Height();

                canvas.DrawText(
                    formatted,
                    canvas.ClipBounds.CenterX() - (textWidth / 2f),
                    canvas.ClipBounds.CenterY() + (textHeight / 2f),
                    paintText
                );

                // Store
                var outputPath = global::System.IO.Path.Combine(ApplicationContext.ExternalCacheDir.AbsolutePath, "smartroadsense_badge.jpg");
                using (var outputStream = new System.IO.FileStream(outputPath, System.IO.FileMode.Create)) {
                    await output.CompressAsync(Bitmap.CompressFormat.Jpeg, 95, outputStream);
                }

                Log.Debug("Written output image to {0}", outputPath);

                // Share
                var i = new Intent(Intent.ActionSend);
                i.SetType("image/jpeg");
                i.AddFlags(ActivityFlags.NewTask);
                i.PutExtra(Intent.ExtraStream, global::Android.Net.Uri.FromFile(new Java.IO.File(outputPath)));
                var shareText = string.Format(GetString(Resource.String.Vernacular_P0_share_badge_text), kms);
                i.PutExtra(Intent.ExtraText, shareText);
                i.PutExtra(Intent.ExtraTitle, shareText);

                var cm = (global::Android.Content.ClipboardManager)GetSystemService(ClipboardService);
                cm.PrimaryClip = ClipData.NewPlainText("share text", shareText);

                Toast.MakeText(ApplicationContext, Resource.String.Vernacular_P0_share_badge_clipboard, ToastLength.Long).Show();

                StartActivity(i);

                Log.Debug("Started sharing activity");
            }
            catch(Exception ex) {
                Log.Error(ex, "Failed preparing sharable bitmap");
                Toast.MakeText(ApplicationContext, Resource.String.Vernacular_P0_share_badge_failure, ToastLength.Short).Show();
            }
            finally {
                if (canvas != null)
                    canvas.Dispose();
                if (output != null)
                    output.Dispose();
                if (source != null)
                    source.Dispose();
            }
        }

    }

}
