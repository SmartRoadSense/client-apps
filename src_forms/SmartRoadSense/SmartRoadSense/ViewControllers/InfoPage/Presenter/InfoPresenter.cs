using System;
namespace SmartRoadSense
{
    public class InfoPresenter : IInfoInputActionPresenter, IInfoOutputActionPresenter, IInfoDataPresenter
    {
        IInfoView _view;
        IInfoRouter _router;
        IInfoInteractor _interactor;

        public InfoPresenter(IInfoView view, MainMasterDetailPage master)
        {
            _view = view;
            _router = new InfoRouter(view, master);
            _interactor = new InfoInteractor(this);
        }
    }
}
