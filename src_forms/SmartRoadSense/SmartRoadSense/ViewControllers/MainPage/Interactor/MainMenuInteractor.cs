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

        public void LaunchService()
        {
            var outcome = new Outcome<string, MainPageException>("hello");
            //var service = new Outcome<string, MainPageException>(new MainPageException("error message"));

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
