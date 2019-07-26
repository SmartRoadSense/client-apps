using System;
namespace SmartRoadSense
{
    public class InfoViewBinder : BaseViewBinder, IInfoView
    {
        IInfoInputActionPresenter _presenter;
        IInfoDataPresenter _dataPresenter;

        public InfoViewBinder(InfoPage page, MainMasterDetailPage master)
        {
            CurrentPage = page;

            var presenter = new InfoPresenter(this, master);
            _presenter = presenter;
            _dataPresenter = presenter;
        }

        MainMasterDetailPage _master;

        public InfoPage CurrentPage { get; }

        // BINDINGS

        // ACTIONS
    }
}
