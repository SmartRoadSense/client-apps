using System;
namespace SmartRoadSense
{
    public class DataPresenter : IDataModelPresenter, IDataInputActionPresenter, IDataOutputActionPresenter
    {
        IDataView _view;
        IDataRouter _router;

        public DataPresenter(IDataView view, MainMasterDetailPage master)
        {
            _view = view;
            _router = new DataRouter(view, master);
        }
    }
}
