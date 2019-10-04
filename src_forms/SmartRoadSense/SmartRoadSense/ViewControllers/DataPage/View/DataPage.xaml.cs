using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SmartRoadSense
{
    public partial class DataPage : ContentPage
    {
        DataViewBinder _viewBinder;

        public DataPage(MainMasterDetailPage master)
        {
            InitializeComponent();

            _viewBinder = new DataViewBinder(this, master);
            BindingContext = _viewBinder;
        }
    }
}
