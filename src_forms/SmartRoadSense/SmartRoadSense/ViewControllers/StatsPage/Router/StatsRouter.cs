using System;
namespace SmartRoadSense
{
    public class StatsRouter : IStatsRouter
    {
        MainMasterDetailPage _master;
        IStatsView _view;

        public StatsRouter(IStatsView view, MainMasterDetailPage master)
        {
            _view = view;
            _master = master;
        }
    }
}
