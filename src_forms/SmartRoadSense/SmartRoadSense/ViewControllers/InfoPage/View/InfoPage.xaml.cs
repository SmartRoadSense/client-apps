using Xamarin.Forms;

namespace SmartRoadSense
{
    public partial class InfoPage : ContentPage
    {
        InfoViewBinder _viewBinder;

        public InfoPage(MainMasterDetailPage master)
        {
            InitializeComponent();

            _viewBinder = new InfoViewBinder(this, master);
            BindingContext = _viewBinder;


        }
    }
}
