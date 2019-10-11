using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SmartRoadSense
{
    public partial class StatsPage : ContentPage
    {
        StatsPageViewBinder _viewBinder;

        public StatsPage(MainMasterDetailPage master)
        {
            InitializeComponent();

            _viewBinder = new StatsPageViewBinder(this, master);
            BindingContext = _viewBinder;
        }
    }
}
