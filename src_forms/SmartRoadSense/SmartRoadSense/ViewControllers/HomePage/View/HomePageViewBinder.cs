using System;
namespace SmartRoadSense
{
    public class HomePageViewBinder : BaseViewBinder, IHomePageView
    {
        IHomePageInputActionPresenter _presenter;

        public HomePageViewBinder(HomePage page, MainMasterDetailPage master)
        {
            CurrentPage = page;

            var presenter = new HomePagePresenter(this, master);
            _presenter = presenter;
        }

        public HomePage CurrentPage { get; }

        // BINDINGS

        // ACTIONS
        public void OpenCarpoolingPopup()
        {
            _presenter.OpenCarpoolingPopup();
        }
    }
}
