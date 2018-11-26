using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SmartRoadSense
{
    public partial class HomePage : ContentPage
    {
        MainMasterDetailPage _master;
        public HomePage(MainMasterDetailPage master)
        {
            InitializeComponent();

            _master = master;
        }
    }
}
