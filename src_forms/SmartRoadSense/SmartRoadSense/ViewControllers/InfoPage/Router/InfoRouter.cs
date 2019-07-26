using System;
namespace SmartRoadSense
{
    public class InfoRouter : IInfoRouter
    {
        IInfoView _view;
        MainMasterDetailPage _master;

        public InfoRouter(IInfoView view, MainMasterDetailPage master)
        {
            _view = view;
            _master = master;
        }
    }
}
