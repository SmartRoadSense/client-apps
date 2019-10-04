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
                new MainMenuItem(typeof(HomePage)){ Id = 0, Title = AppResources.GameTitleLabel, PageArgs = new object[]{master} },
                new MainMenuItem(typeof(HomePage)){ Id = 1, Title = AppResources.LogTitleLabel, PageArgs = new object[]{master} },
                new MainMenuItem(typeof(DataPage)){ Id = 2, Title = AppResources.QueueTitleLabel, PageArgs = new object[]{master} },
                new MainMenuItem(typeof(HomePage)){ Id = 3, Title = AppResources.StatsTitleLabel, PageArgs = new object[]{master} },
                new MainMenuItem(typeof(SettingsPage)){ Id = 4, Title = AppResources.SettingsTitleLabel, PageArgs = new object[]{master} },
                new MainMenuItem(typeof(InfoPage)){ Id = 5, Title = AppResources.AboutTitleLabel, PageArgs = new object[]{master} }
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
