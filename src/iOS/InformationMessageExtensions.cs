using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace SmartRoadSense.iOS {

    public static class InformationMessageExtensions {

		public static UIImage GetIcon(this InformationMessage message) {
            switch (message) {
            case InformationMessage.GpsDisabled:
            case InformationMessage.OutOfCountry:
				return UIImage.FromBundle("ic_location_off_white");

            case InformationMessage.GpsUnfixed:
            case InformationMessage.GpsSuspendedStationary:
			case InformationMessage.GpsSuspendedSpeed:
				return UIImage.FromBundle("ic_location_on_white");

            case InformationMessage.UploadFailure:
				return UIImage.FromBundle("ic_sync_problem_red");

            case InformationMessage.InternalEngineError:
				return UIImage.FromBundle("ic_error_red");

            }
            return null;
        }

        public static string GetTitle(this InformationMessage message) {
            switch (message) {
            case InformationMessage.GpsDisabled:
				return NSBundle.MainBundle.LocalizedString ("Vernacular_P0_information_message_title_gps_off", null);

            case InformationMessage.GpsUnfixed:
				return NSBundle.MainBundle.LocalizedString ("Vernacular_P0_information_message_title_gps_unfixed", null);

			case InformationMessage.GpsSuspendedSpeed:
				return NSBundle.MainBundle.LocalizedString ("Vernacular_P0_information_message_title_gps_suspended_speed", null);

			case InformationMessage.GpsSuspendedStationary:
				return NSBundle.MainBundle.LocalizedString ("Vernacular_P0_information_message_title_gps_suspended_stationary", null);

            case InformationMessage.UploadFailure:
				return NSBundle.MainBundle.LocalizedString ("Vernacular_P0_information_message_title_upload_failure", null);

            case InformationMessage.InternalEngineError:
				return NSBundle.MainBundle.LocalizedString ("Vernacular_P0_information_message_title_engine_error", null);

            case InformationMessage.OutOfCountry:
				return NSBundle.MainBundle.LocalizedString ("Vernacular_P0_information_message_title_out_of_country", null);
            }
            return string.Empty;
        }

        public static string GetDescription(this InformationMessage message) {
            switch (message) {
                case InformationMessage.GpsDisabled:
				return NSBundle.MainBundle.LocalizedString ("Vernacular_P0_information_message_description_gps_off", null);

                case InformationMessage.GpsUnfixed:
				return NSBundle.MainBundle.LocalizedString ("Vernacular_P0_information_message_description_gps_unfixed", null);

                case InformationMessage.GpsSuspendedSpeed:
				return NSBundle.MainBundle.LocalizedString ("Vernacular_P0_information_message_description_gps_suspended_speed", null);

				case InformationMessage.GpsSuspendedStationary:
				return NSBundle.MainBundle.LocalizedString ("Vernacular_P0_information_message_description_gps_suspended_stationary", null);

                case InformationMessage.UploadFailure:
				return NSBundle.MainBundle.LocalizedString ("Vernacular_P0_information_message_description_upload_failure", null);

                case InformationMessage.InternalEngineError:
				return NSBundle.MainBundle.LocalizedString ("Vernacular_P0_information_message_description_engine_error", null);

                case InformationMessage.OutOfCountry:
				return NSBundle.MainBundle.LocalizedString ("Vernacular_P0_information_message_description_out_of_country", null);
            }
            return string.Empty;
        }

		public static UIColor GetTitleColor(this InformationMessage message) {
            switch (message) {
                case InformationMessage.UploadFailure:
                case InformationMessage.InternalEngineError:
				return StyleSettings.ErrorColor ();
                default:
				return StyleSettings.ThemePrimaryColor ();
            }
       	}
    }
}

