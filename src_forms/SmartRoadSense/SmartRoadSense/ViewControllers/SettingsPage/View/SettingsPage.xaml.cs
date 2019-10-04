using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace SmartRoadSense
{
    public partial class SettingsPage : ContentPage
    {
        SettingsViewBinder _viewBinder;

        public SettingsPage(MainMasterDetailPage master)
        {
            InitializeComponent();

            _viewBinder = new SettingsViewBinder(this, master);
            BindingContext = _viewBinder;

            InitializeStrings();
        }

        void InitializeStrings()
        {
            // TODO: get calibration scale factor
             CalibrationScaleFactor.Text = string.Format(AppResources.CalibrationScaleLabel, 982.91f);
            // TODO: get calibration details
            CalibrationDetails.Text = string.Format(AppResources.CalibrationDetailsLabel, 0.998, 0.001);
        }
    }
}
