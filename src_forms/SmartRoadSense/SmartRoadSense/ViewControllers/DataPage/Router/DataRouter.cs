using System;
namespace SmartRoadSense
{
    public class DataRouter : IDataRouter
    {
        IDataView _view;
        MainMasterDetailPage _master;

        public DataRouter(IDataView view, MainMasterDetailPage master)
        {
            _view = view;
            _master = master;
        }
    }
}
