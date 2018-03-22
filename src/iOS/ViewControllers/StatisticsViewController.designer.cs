// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SmartRoadSense.iOS
{
	[Register ("StatisticsViewController")]
	partial class StatisticsViewController
	{
		[Outlet]
		UIKit.UILabel lblLastTrack { get; set; }

		[Outlet]
		UIKit.UILabel lblLastTrackText { get; set; }

		[Outlet]
		UIKit.UILabel lblOverall { get; set; }

		[Outlet]
		UIKit.UILabel lblOverallText { get; set; }

		[Outlet]
		UIKit.UILabel lblWeek { get; set; }

		[Outlet]
		UIKit.UILabel lblWeekText { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (lblLastTrack != null) {
				lblLastTrack.Dispose ();
				lblLastTrack = null;
			}

			if (lblWeek != null) {
				lblWeek.Dispose ();
				lblWeek = null;
			}

			if (lblOverall != null) {
				lblOverall.Dispose ();
				lblOverall = null;
			}

			if (lblLastTrackText != null) {
				lblLastTrackText.Dispose ();
				lblLastTrackText = null;
			}

			if (lblWeekText != null) {
				lblWeekText.Dispose ();
				lblWeekText = null;
			}

			if (lblOverallText != null) {
				lblOverallText.Dispose ();
				lblOverallText = null;
			}
		}
	}
}
