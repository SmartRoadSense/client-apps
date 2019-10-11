using System;
namespace SmartRoadSense
{
    public class StatsPresenter : IStatsDataPresenter, IStatsInputActionPresenter, IStatsOutputActionPresenter
    {
        IStatsView _view;
        IStatsRouter _router;
        IStatsInteractor _interactor;

        public StatsPresenter(IStatsView view, MainMasterDetailPage master)
        {
            _view = view;
            _router = new StatsRouter(view, master);
            _interactor = new StatsInteractor(this);
        }
    }
}
