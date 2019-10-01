using System;
using System.Collections.Generic;

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

        }
    }
}
