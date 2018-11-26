using System;
using System.Collections.ObjectModel;

namespace SmartRoadSense
{
    public class MainMenuViewBinder : BaseViewBinder
    {
        MainPageMenu _page;

        public MainMenuViewBinder(MainPageMenu page)
        {
            _page = page;

            // Populate menu
            MenuItems = new ObservableCollection<MainMenuItem>(new[]
            {
                new MainMenuItem(typeof(HomePage)){ Id = 0, Title = "Home", PageArgs = null },
                new MainMenuItem(typeof(HomePage)){ Id = 1, Title = "Register", PageArgs = null },
                new MainMenuItem(typeof(HomePage)){ Id = 2, Title = "Data", PageArgs = null },
                new MainMenuItem(typeof(HomePage)){ Id = 3, Title = "Stats", PageArgs = null },
                new MainMenuItem(typeof(HomePage)){ Id = 4, Title = "Settings", PageArgs = null },
                new MainMenuItem(typeof(HomePage)){ Id = 5, Title = "Info", PageArgs = null }
            });
        }

        // BINDINGS
        public ObservableCollection<MainMenuItem> MenuItems { get; set; }
    }
}
