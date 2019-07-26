using System;
namespace SmartRoadSense
{
    public class InfoInteractor : IInfoInteractor
    {
        IInfoOutputActionPresenter _presenter;

        public InfoInteractor(IInfoOutputActionPresenter presenter)
        {
            _presenter = presenter;
        }
    }
}
