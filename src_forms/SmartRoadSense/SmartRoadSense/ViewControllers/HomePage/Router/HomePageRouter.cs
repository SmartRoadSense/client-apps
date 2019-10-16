using System;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;

namespace SmartRoadSense
{
    public class HomePageRouter : IHomePageRouter
    {
        IHomePageView _view;
        MainMasterDetailPage _master;
        
        public HomePageRouter(IHomePageView view, MainMasterDetailPage master)
        {
            _view = view;
            _master = master;
        }

        async public Task<bool> OpenCarpoolingPopup()
        {
            try
            {
                var page = new CarpoolingPopupView();
                await _view.CurrentPage.Navigation.PushPopupAsync(page, true);
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"{0} Error pushing popup: {1}", nameof(HomePageRouter), e);
                return false;
            }
        }
    }
}
