using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Support.V4.App;

using SmartRoadSense.Shared;

namespace SmartRoadSense.Android {
    
    [Activity(
        Label = "@string/Vernacular_P0_title_log",
        ParentActivity = typeof(MainActivity)
    )]
    public class LogActivity : AppCompatActivity {

        private class LogAdapter : BaseAdapter {

            private readonly List<UserLog.LogEntry> _data;
            private readonly Context _context;

            public LogAdapter(Context context, IEnumerable<UserLog.LogEntry> data) {
                _context = context;
                _data = new List<UserLog.LogEntry>(data);
            }

            public void AddEntry(UserLog.LogEntry entry) {
                _data.Add(entry);
                NotifyDataSetChanged();
            }

            #region Implemented abstract members of BaseAdapter

            public override Java.Lang.Object GetItem(int position) {
                return null;
            }

            public override long GetItemId(int position) {
                return position;
            }

            public override View GetView(int position, View convertView, ViewGroup parent) {
                if (convertView == null) {
                    var inflater = (LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService);
                    convertView = inflater.Inflate(Resource.Layout.item_log, parent, false);
                }

                var item = _data[position];

                var iconView = convertView.FindViewById<ImageView>(Resource.Id.image);
                if (item.Icon != UserLog.Icon.None) {
                    iconView.Visibility = ViewStates.Visible;
                    switch(item.Icon) {
                        case UserLog.Icon.Warning:
                            iconView.SetImageResource(Resource.Drawable.ic_warning_white_36dp);
                            break;

                        case UserLog.Icon.Error:
                        default:
                            iconView.SetImageResource(Resource.Drawable.ic_error_white_36dp);
                            break;
                    }
                }
                else {
                    iconView.Visibility = ViewStates.Gone;
                }

                var timestampView = convertView.FindViewById<TextView>(Resource.Id.text_timestamp);
                if (item.Timestamp.Date == DateTime.Now.Date) {
                    timestampView.Text = item.Timestamp.ToString("T");
                }
                else {
                    timestampView.Text = item.Timestamp.ToString("G");
                }

                var messageView = convertView.FindViewById<TextView>(Resource.Id.text_message);
                messageView.Text = item.Message;

                return convertView;
            }

            public override int Count {
                get {
                    return _data.Count;
                }
            }

            #endregion

        }

        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.activity_log);

            //Toolbar support
            var toolbar = this.FindViewById<global::Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if(toolbar != null) {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
            if(Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop) {
                FindViewById(Resource.Id.toolbar_shadow).Visibility = ViewStates.Gone;
            }

            UserLog.NewEntryAdded += HandleUserLogNewEntry;

            RefreshLog();
        }

        protected override void OnDestroy() {
            base.OnDestroy();

            UserLog.NewEntryAdded -= HandleUserLogNewEntry;
        }

        public override bool OnCreateOptionsMenu(IMenu menu) {
            MenuInflater.Inflate(Resource.Menu.log, menu);
            
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item) {
            if(item == null)
                return false;
            
            switch (item.ItemId) {
                case global::Android.Resource.Id.Home:
                    NavUtils.NavigateUpFromSameTask(this);

                    return true;

                case Resource.Id.action_refresh_log:
                    RefreshLog();

                    return true;

                case Resource.Id.action_clear_log:
                    UserLog.Clear();
                    RefreshLog();

                    return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void RefreshLog() {
            var list = this.FindViewById<ListView>(Resource.Id.list_log);
            list.Adapter = new LogAdapter(this, UserLog.Entries);

            var textCount = this.FindViewById<TextView>(Resource.Id.text_log_count);
            textCount.Text = string.Format(GetString(Resource.String.Vernacular_P0_log_entries_count), UserLog.Count);
        }

        private void HandleUserLogNewEntry(object sender, UserLog.NewEntryEventArgs e) {
            RunOnUiThread(() => {
                var list = FindViewById<ListView>(Resource.Id.list_log);
                var adapter = (LogAdapter)list.Adapter;
                (adapter).AddEntry(e.Entry);

                var textCount = this.FindViewById<TextView>(Resource.Id.text_log_count);
                textCount.Text = string.Format(GetString(Resource.String.Vernacular_P0_log_entries_count), UserLog.Count);
            });
        }

    }

}
