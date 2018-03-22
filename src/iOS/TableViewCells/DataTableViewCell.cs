
using System;

using Foundation;
using UIKit;
using SmartRoadSense.Shared;
using SmartRoadSense.Shared.ViewModel;

namespace SmartRoadSense.iOS
{
	public partial class DataTableViewCell : UITableViewCell
	{
		public DataTableViewCell () : base()
		{
		}

		public DataTableViewCell (IntPtr handle) : base(handle)
		{
		}

		public void UpdateCell(UploadQueueViewModel.UploadQueueItem data){

			this.BackgroundColor = StyleSettings.ThemePrimaryDarkLightenedColor ();

			lblFileName.Text = data.Filename;
			lblFileName.TextColor = StyleSettings.TextOnDarkColor ();

			lblRecorded.Text = NSBundle.MainBundle.LocalizedString("Vernacular_P0_label_file_creation", null).PrepareForLabel ();
			lblRecorded.TextColor = StyleSettings.SubtleTextOnBrightColor ();
		
			lblDate.Text = data.Created.ToString("ddd dd MMMMM yyyy hh:mm");
			lblDate.TextColor = StyleSettings.TextOnDarkColor ();
		
			lblSize.Text = NSBundle.MainBundle.LocalizedString("Vernacular_P0_label_file_size", null).PrepareForLabel ();
			lblSize.TextColor = StyleSettings.SubtleTextOnBrightColor ();

			int ksize = (int)Math.Ceiling(data.FileSize / 1024.0);

			lblSizeData.Text = string.Format(NSBundle.MainBundle.LocalizedString("Vernacular_P0_label_file_size_value", null).PrepareForLabel (), ksize);
			lblSizeData.TextColor = StyleSettings.TextOnDarkColor ();
		
		}
	}
}

