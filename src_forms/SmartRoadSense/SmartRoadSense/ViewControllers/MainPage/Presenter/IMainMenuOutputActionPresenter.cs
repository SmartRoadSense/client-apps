using System;
namespace SmartRoadSense
{
    public interface IMainMenuOutputActionPresenter
    {
        void LaunchServiceActionSuccess(string message);
        void LaunchServiceActionError(MainPageException error);
    }
}
