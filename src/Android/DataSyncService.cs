using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Android.App;
using Android.App.Job;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using SmartRoadSense.Shared;
using SmartRoadSense.Shared.Data;

namespace SmartRoadSense.Android {
    
    [Service(
        Enabled = true,
        Exported = true,
        Label = "@string/Vernacular_P0_service_sync"
    )]
    public class DataSyncService : IntentService {

        public override void OnCreate() {
            base.OnCreate();

            Log.Debug("DataSync service created");
        }

        protected async override void OnHandleIntent(Intent intent) {
            Log.Debug("Handling synchronization request");

            using (var cancellationSource = new CancellationTokenSource()) {
                Log.Debug("Sync service running");

                var syncResult = await App.Sync.Synchronize(cancellationSource.Token);

                Log.Debug("Sync service completing with {0}: {1} files uploaded and {2} deleted",
                    (syncResult.HasFailed) ? "failure" : "success",
                    syncResult.DataPiecesUploaded,
                    syncResult.DataPiecesDeleted);
            }

            DataSyncReceiver.CompleteWakefulIntent(intent);
        }

    }

}
