using System;
using Xamarin.Forms;

namespace SmartRoadSense
{
    public class MainMasterDetailPage : MasterDetailPage
    {
        MainPageMenu _mainPageMenu;

        public MainMasterDetailPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            IsGestureEnabled = false;

            _mainPageMenu = new MainPageMenu(this) { Title = "Menu" };
            Master = _mainPageMenu;
            Detail = new NavigationPage(new HomePage(this));

            MasterBehavior = MasterBehavior.Popover;
            Master.BackgroundColor = Color.Transparent;

            _mainPageMenu.ListView.ItemSelected += ListView_ItemSelected;
        }

        public void GoToHomePage()
        {
            Detail = new NavigationPage(new HomePage(this));
        }

        public void CloseMenu() => IsPresented = false;
        public void OpenMenu() => IsPresented = true;
        public void ToggleMenu() => IsPresented = !IsPresented;

        async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(e.SelectedItem is MainMenuItem item))
                return;

            if (item.TargetType != null)
            {
                var page = (Page)Activator.CreateInstance(item.TargetType, item.PageArgs);
                page.Title = item.Title;
                await Detail.Navigation.PushAsync(page);
            }

            IsPresented = false;
            _mainPageMenu.ListView.SelectedItem = null;
        }
    }
}
