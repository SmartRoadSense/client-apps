using System;
namespace SmartRoadSense
{
    public class MainMenuInteractor : IMainMenuInteractor
    {
        IMainMenuOutputActionPresenter _presenter;
        public MainMenuInteractor(IMainMenuOutputActionPresenter presenter)
        {
            _presenter = presenter;
        }

        public async void LaunchService()
        {
            var outcome = await RemoteDataManager.Instance.TrackDataService.SendTracks("tracks");

            switch(outcome.Event)
            {
                case OutcomeEvents.success:
                    _presenter.LaunchServiceActionSuccess(outcome.Element);
                    break;
                case OutcomeEvents.error:
                    _presenter.LaunchServiceActionError(outcome.Error);
                    break;
            }
        }
    }
}
