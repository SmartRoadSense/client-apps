using System;
using System.Collections.ObjectModel;

namespace SmartRoadSense
{
    public class MainMenuViewBinder : BaseViewBinder, IMainMenuView
    {
        IMainMenuInputActionPresenter _presenter;
        IMainMenuDataPresenter _dataPresenter;

        public MainMenuViewBinder(MainPageMenu page, MainMasterDetailPage master)
        {
            CurrentPage = page;
            var presenter = new MainMenuPresenter(this, master);
            _presenter = presenter;
            _dataPresenter = presenter;

            // Populate menu
            MenuItems = new ObservableCollection<MainMenuItem>(new[]
            {
                new MainMenuItem(typeof(HomePage)){ Id = 0, Title = "Home", PageArgs = new object[]{master} },
                new MainMenuItem(typeof(HomePage)){ Id = 1, Title = "Register", PageArgs = new object[]{master} },
                new MainMenuItem(typeof(HomePage)){ Id = 2, Title = "Data", PageArgs = new object[]{master} },
                new MainMenuItem(typeof(HomePage)){ Id = 3, Title = "Stats", PageArgs = new object[]{master} },
                new MainMenuItem(typeof(HomePage)){ Id = 4, Title = "Settings", PageArgs = new object[]{master} },
                new MainMenuItem(typeof(HomePage)){ Id = 5, Title = "Info", PageArgs = new object[]{master} }
            });
        }

        // BINDINGS
        public MainPageMenu CurrentPage { get; }

        public ObservableCollection<MainMenuItem> MenuItems
        {
            get => _dataPresenter.MenuItems;
            set
            {
                _dataPresenter.MenuItems = value;
                OnPropertyChanged();
            }
        }

        // Actions
        public void GoHome()
        {
            _presenter.ActionGoHome();
        }
    }
}
