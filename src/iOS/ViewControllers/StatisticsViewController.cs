using System;
using Foundation;
using SmartRoadSense.Shared;
using SmartRoadSense.Shared.Database;
using UIKit;

namespace SmartRoadSense.iOS
{
    public partial class StatisticsViewController : UIViewController
    {
		public StatisticsViewController(IntPtr handle) : base (handle)
        {
			this.Title = NSBundle.MainBundle.LocalizedString("Vernacular_P0_title_stats", null).PrepareForLabel();
		}

		public override void DidReceiveMemoryWarning()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.

			String weekText = NSBundle.MainBundle.LocalizedString("Vernacular_P0_stats_week_label", null).PrepareForLabel();
            lblWeekText.Text = weekText;
            lblWeekText.TextColor = StyleSettings.TextOnDarkColor();

			String overallText = NSBundle.MainBundle.LocalizedString("Vernacular_P0_stats_overall_label", null).PrepareForLabel();
			lblOverallText.Text = overallText;
			lblOverallText.TextColor = StyleSettings.TextOnDarkColor();

			String lastTrackText = NSBundle.MainBundle.LocalizedString("Vernacular_P0_stats_last_track_label", null).PrepareForLabel();
			lblLastTrackText.Text = lastTrackText;
            lblLastTrackText.TextColor = StyleSettings.TextOnDarkColor();

			try
			{
				using (var conn = DatabaseUtility.OpenConnection())
				{
					var week = StatisticHelper.GetPeriodSummary(conn, StatisticPeriod.Week);
					var overall = StatisticHelper.GetPeriodSummary(conn, StatisticPeriod.Overall);
					var last = StatisticHelper.GetLastTrack(conn);

					UpdateKmCounter(lblLastTrack, last?.DistanceTraveled);
					UpdateKmCounter(lblWeek, week.Distance);
					UpdateKmCounter(lblOverall, overall.Distance);
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Failed to load statistics");
			}

            // TODO: add share button
		}

        private void UpdateKmCounter(UILabel label, double? kms)
		{
			var integer = (kms.HasValue) ? kms.Value.ToString("0.") : "0";

            string formatted = NSBundle.MainBundle.LocalizedString("Vernacular_P0_stats_kms_value_default", null);
			if (kms.HasValue)
                formatted = string.Format(NSBundle.MainBundle.LocalizedString("Vernacular_P0_stats_kms_value_format", null).PrepareForLabel(), kms);
            label.Text = formatted;
		}
    }
}

