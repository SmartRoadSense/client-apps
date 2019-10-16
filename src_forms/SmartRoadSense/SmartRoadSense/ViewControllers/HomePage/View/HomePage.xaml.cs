using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SmartRoadSense
{
    public partial class HomePage : ContentPage
    {
        MainMasterDetailPage _master;
        HomePageViewBinder _viewBinder;

        public HomePage(MainMasterDetailPage master)
        {
            InitializeComponent();

            _master = master;
            _viewBinder = new HomePageViewBinder(this, master);
            BindingContext = _viewBinder;

            CarButton.Clicked += CarButton_Clicked;
        }

        private void CarButton_Clicked(object sender, EventArgs e)
        {
            _viewBinder.OpenCarpoolingPopup();
        }
    }
}
