
using System;

using Foundation;
using UIKit;
using System.Collections.Generic;
using SmartRoadSense.Shared;

namespace SmartRoadSense.iOS
{
	public partial class LogTableViewCell : UITableViewCell
	{
		public LogTableViewCell() : base()
		{
		}

		public LogTableViewCell (IntPtr handle) : base(handle)
		{
		}

		public void UpdateCell(UserLog.LogEntry data){

			this.BackgroundColor = StyleSettings.ThemePrimaryDarkLightenedColor ();

			lblTime.Text = data.Timestamp.ToString ("T");
			lblTime.TextColor = StyleSettings.ThemePrimaryColor ();

			lblText.Text = data.Message;
			lblText.TextColor = StyleSettings.TextOnDarkColor ();

			if (data.Icon != UserLog.Icon.None) {
				imgAlert.Hidden = false;
				switch (data.Icon) {
				case UserLog.Icon.Warning:
					imgAlert.Image = UIImage.FromBundle ("ic_warning_white");
					break;
				case UserLog.Icon.Error:
					imgAlert.Image = UIImage.FromBundle ("ic_error_white");
					break;
				default:
					imgAlert.Image = UIImage.FromBundle ("ic_error_white");
					break;
				}
			} else {
				imgAlert.Hidden = true;
			}
		}
	}
}

