using System;
namespace SmartRoadSense
{
    public class HomePageViewBinder : BaseViewBinder, IHomePageView
    {
        IHomePageInputActionPresenter _presenter;

        // SRS engine
        RecordingViewModel RVM;

        public HomePageViewBinder(HomePage page, MainMasterDetailPage master)
        {
            CurrentPage = page;

            var presenter = new HomePagePresenter(this, master);
            _presenter = presenter;

            // Initialize Recorder
            RVM = new RecordingViewModel();
        }

        public HomePage CurrentPage { get; }

        // BINDINGS

        // ACTIONS
        public void OpenCarpoolingPopup()
        {
            if (RVM.IsRecording)
                return;

            // TODO: reactivate after calibration has been implemented
            //if (SettingsManager.Instance.CalibrationDone)
            //{
                _presenter.OpenCarpoolingPopup();
            //}
            //else
            //{
                // TODO: warn user that calibration has to be done
            //}
        }
    }
}
