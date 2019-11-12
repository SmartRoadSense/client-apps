using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmartRoadSense {

    public class UploadQueueViewModel : BaseViewModel {

        /// <summary>
        /// Represents an item of the upload queue.
        /// </summary>
        public class UploadQueueItem {

			public UploadQueueItem(FileSystemToken token) {
				Filename = token.Filename;
				Created = token.LocalCreationTime;
				FileSize = token.Size;
            }

			/// <summary>
			/// Name of the file.
			/// </summary>
			/// <value>The filename.</value>
            public string Filename { get; private set; }

			/// <summary>
			/// Creation time of the file, in local timezone.
			/// </summary>
            public DateTime Created { get; private set; }

			/// <summary>
			/// Gets the size of the file in bytes.
			/// </summary>
            public int FileSize { get; private set; }

        }

        private readonly Recorder _recorder;

        public UploadQueueViewModel() {
            _recorder = App.Recorder;
            _queue = new ObservableCollection<UploadQueueItem>();

            RefreshQueueCommand = new RelayCommand(HandleRefreshQueueCommand);
            ClearUploadQueueCommand = new RelayCommand(HandleClearUploadQueueCommand);
            DeleteUploadQueueItemCommand = new RelayCommand<UploadQueueItem>(HandleDeleteUploadQueueItemCommand);
            ForceQueueUploadCommand = new RelayCommand(HandleForceQueueUploadCommand);
        }

        #pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        private async void HandleRefreshQueueCommand()
        {
            await RefreshUploadQueue();
        }

        private async void HandleClearUploadQueueCommand() {
            await DataStore.DeleteAll();

            await RefreshUploadQueue();
        }

        private async void HandleDeleteUploadQueueItemCommand(UploadQueueItem item) {
            //TODO: fix this
            //await App.Sync.Delete(item.Filename);

            await RefreshUploadQueue();
        }

        private async void HandleForceQueueUploadCommand() {
            using (var tokenSource = new CancellationTokenSource()) {
                //TODO: add "cancel" button to cancel synchronization
                await App.Sync.Synchronize(tokenSource.Token, SyncPolicy.Forced);
            }
        }
        #pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void

        public override void OnCreate() {
            _recorder.DataFileWritten += HandleRecorderDataFileWritten;
            App.Sync.StatusChanged += HandleSyncManagerStatusChanged;
            App.Sync.SyncError += HandleSyncManagerSyncError;
        }

        public override void OnDestroy() {
            _recorder.DataFileWritten -= HandleRecorderDataFileWritten;
            App.Sync.StatusChanged -= HandleSyncManagerStatusChanged;
            App.Sync.SyncError -= HandleSyncManagerSyncError;
        }

        private async void HandleRecorderDataFileWritten(object sender, FileGeneratedEventArgs e) {
            //New files are always on bottom, so just add it to the queue
			var token = await FileOperations.GetToken(e.Filepath);
            _queue.Add(new UploadQueueItem(token));
        }

        private void HandleSyncManagerStatusChanged(object sender, EventArgs e) {
            OnPropertyChanged(() => IsUploading);
            IsUploadingChanged.Raise(this);

            if (!App.Sync.IsSyncing) {
                //Manager stopped syncing, files might have changed
                RefreshUploadQueue().Forget();
            }
        }

        private void HandleSyncManagerSyncError(object sender, SyncErrorEventArgs e) {
            SyncErrorReported.Raise(this, e);
        }

        private async Task RefreshUploadQueue() {
            var files = await FileOperations.EnumerateFolderAsync(FileNaming.DataQueuePath, string.Empty);

			_queue = new ObservableCollection<UploadQueueItem>(from f in files
															   select new UploadQueueItem(f));

            OnPropertyChanged(() => UploadQueue);
            UploadQueueUpdated.Raise(this);
        }

        #region Bindable properties

        private ObservableCollection<UploadQueueItem> _queue;

        public ReadOnlyObservableCollection<UploadQueueItem> UploadQueue {
            get {
                return new ReadOnlyObservableCollection<UploadQueueItem>(_queue);
            }
        }

        public bool IsUploading {
            get {
                return App.Sync.IsSyncing;
            }
        }

        #endregion

        #region Manual events

        public event EventHandler UploadQueueUpdated;

        public event EventHandler IsUploadingChanged;

        public event EventHandler<SyncErrorEventArgs> SyncErrorReported;

        #endregion

        #region Commands

        public ICommand RefreshQueueCommand;

        /// <summary>
        /// Command that clears the data queue completely.
        /// </summary>
        public ICommand ClearUploadQueueCommand;

        /// <summary>
        /// Command that deletes a single queue item.
        /// Takes a UploadQueueItem instance as parameter.
        /// </summary>
        public ICommand DeleteUploadQueueItemCommand;

        /// <summary>
        /// Command that forces a synchronization pass.
        /// </summary>
        public ICommand ForceQueueUploadCommand;

        #endregion

    }

}

