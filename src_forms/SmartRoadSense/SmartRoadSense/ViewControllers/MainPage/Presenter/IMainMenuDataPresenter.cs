using System.Collections.ObjectModel;

namespace SmartRoadSense
{
    public interface IMainMenuDataPresenter
    {
        ObservableCollection<MainMenuItem> MenuItems { get; set; }
    }
}
