using System;
namespace SmartRoadSense
{
    public class HomePageInteractor : IHomePageInteractor
    {
        IHomePageOutputActionPresenter _presenter;

        public HomePageInteractor(IHomePageOutputActionPresenter presenter)
        {
            _presenter = presenter;
        }
    }
}
