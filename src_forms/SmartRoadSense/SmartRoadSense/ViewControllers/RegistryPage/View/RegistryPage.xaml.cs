using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace SmartRoadSense
{
    public partial class RegistryPage : ContentPage
    {
        RegistryViewBinder _viewBinder;

        public RegistryPage(MainMasterDetailPage master)
        {
            InitializeComponent();

            _viewBinder = new RegistryViewBinder(this, master);
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
