using System;
namespace SmartRoadSense
{
    public class StatsPageViewBinder : BaseViewBinder, IStatsView
    {
        MainMasterDetailPage _master;
        IStatsInputActionPresenter _presenter;
        IStatsDataPresenter _dataPresenter;

        public StatsPageViewBinder(StatsPage page, MainMasterDetailPage master)
        {
            CurrentPage = page;
            _master = master;

            var presenter = new StatsPresenter(this, master);
            _presenter = presenter;
            _dataPresenter = presenter;
        }

        public StatsPage CurrentPage { get; }

        // BINDINGS
        public string LastTrackText
        {
            get => true ? AppResources.KmsValueDefaultStatsLabel : string.Format(AppResources.KmsValueFormatStatsLabel, "0.0");
        }

        public string LastWeekTracksText
        {
            get => true ? AppResources.KmsValueDefaultStatsLabel : string.Format(AppResources.KmsValueFormatStatsLabel, "0.0");
        }

        public string OverallTracksText
        {
            get => true ? AppResources.KmsValueDefaultStatsLabel : string.Format(AppResources.KmsValueFormatStatsLabel, "0.0");
        }


    }
}
