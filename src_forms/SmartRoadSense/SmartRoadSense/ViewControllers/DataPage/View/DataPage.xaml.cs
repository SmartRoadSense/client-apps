﻿using System;
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

        public void Handler_DeleteClicked(object s, EventArgs e)
        {
            // TODO
        }

        public void Handler_UploadClicked(object s, EventArgs e)
        {
            // TODO
        }
    }

    

    
}