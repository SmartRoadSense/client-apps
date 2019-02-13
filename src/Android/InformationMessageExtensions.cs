using System;
using System.Collections.Generic;

using Android.Content;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace SmartRoadSense.Android {

    public static class InformationMessageExtensions {

        public static Drawable GetIcon(this InformationMessage message, Context context) {
            switch (message) {
                case InformationMessage.GpsDisabled:
                case InformationMessage.OutOfCountry:
                    return context.Resources.GetDrawable(Resource.Drawable.ic_location_off_white_36dp);

                case InformationMessage.GpsUnfixed:
                case InformationMessage.GpsSuspendedStationary:
				case InformationMessage.GpsSuspendedSpeed:
                    return context.Resources.GetDrawable(Resource.Drawable.ic_location_on_white_36dp);

                case InformationMessage.UploadFailure:
                    return context.Resources.GetDrawable(Resource.Drawable.ic_sync_problem_red_36dp);

                case InformationMessage.InternalEngineError:
                    return context.Resources.GetDrawable(Resource.Drawable.ic_error_red_36dp);

                case InformationMessage.Syncing:
                    return context.Resources.GetDrawable(Resource.Drawable.ic_sync_white_36dp);
            }

            return null;
        }

        public static string GetTitle(this InformationMessage message, Context context) {
            switch (message) {
                case InformationMessage.GpsDisabled:
                    return context.GetString(Resource.String.Vernacular_P0_information_message_title_gps_off);

                case InformationMessage.GpsUnfixed:
                    return context.GetString(Resource.String.Vernacular_P0_information_message_title_gps_unfixed);

                case InformationMessage.GpsSuspendedStationary:
					return context.GetString(Resource.String.Vernacular_P0_information_message_title_gps_suspended_stationary);

				case InformationMessage.GpsSuspendedSpeed:
					return context.GetString(Resource.String.Vernacular_P0_information_message_title_gps_suspended_speed);

                case InformationMessage.UploadFailure:
                    return context.GetString(Resource.String.Vernacular_P0_information_message_title_upload_failure);

                case InformationMessage.InternalEngineError:
                    return context.GetString(Resource.String.Vernacular_P0_information_message_title_engine_error);

                case InformationMessage.OutOfCountry:
                    return context.GetString(Resource.String.Vernacular_P0_information_message_title_out_of_country);

                case InformationMessage.Syncing:
                    return context.GetString(Resource.String.Vernacular_P0_information_message_title_syncing);
            }

            return string.Empty;
        }

        public static string GetDescription(this InformationMessage message, Context context) {
            switch (message) {
                case InformationMessage.GpsDisabled:
                    return context.GetString(Resource.String.Vernacular_P0_information_message_description_gps_off);

                case InformationMessage.GpsUnfixed:
                    return context.GetString(Resource.String.Vernacular_P0_information_message_description_gps_unfixed);

                case InformationMessage.GpsSuspendedStationary:
                    return context.GetString(Resource.String.Vernacular_P0_information_message_description_gps_suspended_stationary);

				case InformationMessage.GpsSuspendedSpeed:
					return context.GetString(Resource.String.Vernacular_P0_information_message_description_gps_suspended_speed);

                case InformationMessage.UploadFailure:
                    return context.GetString(Resource.String.Vernacular_P0_information_message_description_upload_failure);

                case InformationMessage.InternalEngineError:
                    return context.GetString(Resource.String.Vernacular_P0_information_message_description_engine_error);

                case InformationMessage.OutOfCountry:
                    return context.GetString(Resource.String.Vernacular_P0_information_message_description_out_of_country);

                case InformationMessage.Syncing:
                    return context.GetString(Resource.String.Vernacular_P0_information_message_description_syncing);
            }

            return string.Empty;
        }

        public static Color GetTitleColor(this InformationMessage message, Context context) {
            switch (message) {
                case InformationMessage.UploadFailure:
                case InformationMessage.InternalEngineError:
                    return context.Resources.GetColor(Resource.Color.error);

                default:
                    return context.Resources.GetColor(Resource.Color.theme_primary);
            }
        }

    }

}

