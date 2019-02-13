using System;
using System.Threading;
using System.Threading.Tasks;

using Android.App;
using Android.App.Job;
using Android.Content;

using SmartRoadSense.Shared;

namespace SmartRoadSense.Android {

    [Service(
        Enabled = true,
        Exported = true,
        Label = "@string/Vernacular_P0_service_sync",
        Permission = JobService.PermissionBind
    )]
    public class DataSyncJobService : JobService {

        public ComponentName ComponentName {
            get {
                return new ComponentName(App.Context, Class.Name);
            }
        }

        public override void OnCreate() {
            base.OnCreate();

            Log.Debug("DataSync job created");
        }

        private CancellationTokenSource _cancellationSource;

        #region Implemented abstract members of JobService

        public override bool OnStartJob(JobParameters @params) {
            Log.Debug("Start job command received (job {0})", @params.JobId);

            //Launch background thread for processing
            JobHandler(@params).Forget();

            return true;
        }

        public override bool OnStopJob(JobParameters @params) {
            Log.Debug("Stop job command received (job {0})", @params.JobId);

            if (_cancellationSource != null) {
                Log.Debug("Canceling running job");

                _cancellationSource.Cancel();

                return true; //reschedules
            }
            else {
                Log.Debug("Cannot cancel sync job (null cancellation source)");

                return false;
            }
        }

        #endregion Implemented abstract members of JobService

        private async Task JobHandler(JobParameters @params) {
            if (_cancellationSource != null) {
                Log.Warning(new ArgumentException(nameof(_cancellationSource)), "Cancellation token source non-null on start job");
                _cancellationSource.Cancel();
            }

            try {
                Log.Debug("Sync job handler running");

                _cancellationSource = new CancellationTokenSource();

                var syncResult = await App.Sync.Synchronize(_cancellationSource.Token);

                Log.Debug("Sync job completing with {0}: {1} files uploaded and {2} deleted",
                    (syncResult.HasFailed) ? "failure" : "success",
                    syncResult.DataPiecesUploaded,
                    syncResult.DataPiecesDeleted);

                JobFinished(@params, false);
            }
            finally {
                _cancellationSource = null;
            }

            Log.Debug("Sync job handler terminated");
        }

    }

}
