using System;
namespace SmartRoadSense
{
    public class DataViewBinder : BaseViewBinder, IDataView
    {
        MainMasterDetailPage _master;
        IDataModelPresenter _dataPresenter;
        IDataInputActionPresenter _presenter;

        public DataViewBinder(DataPage page, MainMasterDetailPage master)
        {
            CurrentPage = page;
            _master = master;

            var presenter = new DataPresenter(this, master);
            _dataPresenter = presenter;
            _presenter = presenter;
        }

        public DataPage CurrentPage { get; }

        // BINDINGS
        public bool UploadDataIsPresent
        {
            get => false; // TODO: get value dinamically
        }
    }
}
