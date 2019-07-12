using Xamarin.Forms;

namespace SmartRoadSense
{
    public partial class MainPageMenu : ContentPage
    {
        public ListView ListView;
        public MainMasterDetailPage Master;

        MainMenuViewBinder _viewBinder;

        public MainPageMenu(MainMasterDetailPage master)
        {
            InitializeComponent();

            Master = master;
            _viewBinder = new MainMenuViewBinder(this, master);

            BindingContext = _viewBinder;
            ListView = MenuItemsListView;
        }
    }
}
