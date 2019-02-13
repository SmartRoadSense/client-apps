using System;
using Android.Support.V7.App;
using SmartRoadSense.Shared.ViewModel;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Android.Content;
using System.Collections.Generic;
using Android.App;

namespace SmartRoadSense.Android {

    [Activity(
        Label = "@string/Vernacular_P0_title_queue",
        ParentActivity = typeof(MainActivity)
    )]
    public class QueueActivity : AppCompatActivity {

        protected UploadQueueViewModel ViewModel { get; private set; }

        private Button _buttonUpload;
        private ListView _listFiles;

        private MessageSnackbarDisplayer _bottomDisplayer;

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_queue);

            //Toolbar support
            var toolbar = this.FindViewById<global::Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if(toolbar != null) {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
            if(Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
                this.FindViewById(Resource.Id.toolbar_shadow).Visibility = ViewStates.Gone;
            }

            _buttonUpload = this.FindViewById<Button>(Resource.Id.button_force_upload);
            _buttonUpload.Click += HandleForceUploadClicked;

            _listFiles = this.FindViewById<ListView>(Resource.Id.listview_file_queue);
            //TODO reinstitute single item deletion
            //_listFiles.ItemLongClick += HandleFileListLongClick;

            _bottomDisplayer = new MessageSnackbarDisplayer(this, FindViewById<View>(Resource.Id.snackbar_container), null);

            //View model setup
            ViewModel = new UploadQueueViewModel();
            ViewModel.OnCreate();

            ViewModel.UploadQueueUpdated += HandleUploadQueueUpdated;
            ViewModel.IsUploadingChanged += HandleIsUploadingChanged;
            ViewModel.SyncErrorReported += HandleSyncErrorReported;
        }

        protected override void OnResume() {
            base.OnResume();

            ViewModel.RefreshQueueCommand.Execute(null);
            RefreshUploadingUi();
        }

        protected override void OnDestroy() {
            base.OnDestroy();

            if (ViewModel != null) {
                ViewModel.UploadQueueUpdated -= HandleUploadQueueUpdated;
                ViewModel.IsUploadingChanged -= HandleIsUploadingChanged;
                ViewModel.SyncErrorReported -= HandleSyncErrorReported;

                ViewModel.OnDestroy();
                ViewModel = null;
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu) {
            MenuInflater.Inflate(Resource.Menu.queue, menu);

            return true;
        }

        public override bool OnPrepareOptionsMenu(IMenu menu) {
            //Enable clear action only when items available
            menu.FindItem(Resource.Id.action_clear_queue).SetEnabled(ViewModel.UploadQueue.Count > 0);

            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item) {
            if(item == null)
                return false;
            
            switch (item.ItemId) {
                case global::Android.Resource.Id.Home:
                    NavUtils.NavigateUpFromSameTask(this);
                    return true;

                case Resource.Id.action_clear_queue:
                    HandleClearQueueClick();
                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void HandleUploadQueueUpdated(object sender, EventArgs e) {
            RunOnUiThread(() => {
                RefreshList();

                InvalidateOptionsMenu();
            });
        }

        private void HandleIsUploadingChanged(object sender, EventArgs e) {
            RunOnUiThread(() => {
                RefreshUploadingUi();
            });
        }

        private void HandleSyncErrorReported(object sender, SmartRoadSense.Shared.SyncErrorEventArgs e) {
            RunOnUiThread(() => {
                _bottomDisplayer.Show(InformationMessage.UploadFailure, MessageSnackbarDisplayer.LongDuration);
            });
        }

        private void HandleForceUploadClicked(object sender, EventArgs e) {
            ViewModel.ForceQueueUploadCommand.Execute(null);
        }

        private void HandleClearQueueClick() {
            new global::Android.Support.V7.App.AlertDialog.Builder(this)
                .SetTitle(Resource.String.Vernacular_P0_dialog_queue_clear_title)
                .SetMessage(Resource.String.Vernacular_P0_dialog_queue_clear_description)
                .SetPositiveButton(Resource.String.Vernacular_P0_dialog_queue_clear_delete_all, (s, args) => {
                    ViewModel.ClearUploadQueueCommand.Execute(null);
                })
                .SetNegativeButton(Resource.String.Vernacular_P0_dialog_cancel, (EventHandler<DialogClickEventArgs>)null)
                .Show();
        }

        private void HandleFileListLongClick(object sender, AdapterView.ItemLongClickEventArgs e) {
            if (e.Position < 0 || e.Position > _listFiles.Adapter.Count)
                return;
            
            var item = ((QueueItemAdapter)_listFiles.Adapter)[e.Position];

            new global::Android.Support.V7.App.AlertDialog.Builder(this)
                .SetTitle(Resource.String.Vernacular_P0_dialog_queue_file_title)
                .SetPositiveButton(Resource.String.Vernacular_P0_dialog_queue_file_delete, (s, args) => {
                    ViewModel.DeleteUploadQueueItemCommand.Execute(item);
                })
                .SetNegativeButton(Resource.String.Vernacular_P0_dialog_queue_file_cancel, (EventHandler<DialogClickEventArgs>)null)
                .Show();
        }

        private void RefreshUploadingUi() {
            _buttonUpload.Enabled = !ViewModel.IsUploading;

            _bottomDisplayer.ShowIf(InformationMessage.Syncing, ViewModel.IsUploading);
        }

        private void RefreshList() {
            if (ViewModel.UploadQueue.Count > 0) {
                this.FindViewById(Resource.Id.text_no_files_in_queue).Visibility = ViewStates.Gone;

                _listFiles.Adapter = new QueueItemAdapter(this, ViewModel.UploadQueue);
                _listFiles.Visibility = ViewStates.Visible;

                _buttonUpload.Visibility = ViewStates.Visible;
            }
            else {
                this.FindViewById(Resource.Id.text_no_files_in_queue).Visibility = ViewStates.Visible;

                _listFiles.Visibility = ViewStates.Gone;

                _buttonUpload.Visibility = ViewStates.Gone;
            }
        }

        private class QueueItemAdapter : ListAdapter<UploadQueueViewModel.UploadQueueItem> {

            public QueueItemAdapter(Context context, IReadOnlyList<UploadQueueViewModel.UploadQueueItem> data)
                : base(context, data) {

            }

            protected override View CreateView(int position, ViewGroup parent, LayoutInflater inflater) {
                return inflater.Inflate(Resource.Layout.item_queue, parent, false);
            }

            protected override void UpdateView(int position, View view, UploadQueueViewModel.UploadQueueItem data) {
                view.FindViewById<TextView>(Resource.Id.text_filename).Text = data.Filename;

                var creationString = data.Created.ToString("ddd dd MMMMM yyyy HH:mm");
                view.FindViewById<TextView>(Resource.Id.text_file_creation).Text = creationString;

                int ksize = (int)Math.Ceiling(data.FileSize / 1024.0);
                view.FindViewById<TextView>(Resource.Id.text_file_size).Text = string.Format(
                    Context.GetString(Resource.String.Vernacular_P0_label_file_size_value),
                    ksize);
            }

        }

    }

}

