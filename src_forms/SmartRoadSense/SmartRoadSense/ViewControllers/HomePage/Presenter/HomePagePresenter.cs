using System;
namespace SmartRoadSense
{
    public class HomePagePresenter : IHomePageInputActionPresenter, IHomePageOutputActionPresenter
    {
        IHomePageView _view;
        IHomePageRouter _router;
        IHomePageInteractor _interactor;

        public HomePagePresenter(IHomePageView view, MainMasterDetailPage master)
        {
            _view = view;
            _router = new HomePageRouter(view, master);
            _interactor = new HomePageInteractor(this);
        }

        public void OpenCarpoolingPopup()
        {
            _router.OpenCarpoolingPopup();
        }
    }
}
