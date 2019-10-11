using System;
namespace SmartRoadSense
{
    public class StatsInteractor : IStatsInteractor
    {
        IStatsOutputActionPresenter _presenter;

        public StatsInteractor(IStatsOutputActionPresenter presenter)
        {
            _presenter = presenter;
        }
    }
}
