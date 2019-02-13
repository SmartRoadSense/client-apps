using System;
using Android.Widget;
using Android.Views;
using Android.Content;
using System.Collections.Generic;

namespace SmartRoadSense.Android {

    /// <summary>
    /// Custom array adapter.
    /// </summary>
    public abstract class ListAdapter<T> : BaseAdapter<T> {

        private readonly Context _context;

        private IReadOnlyList<T> _data;

        public ListAdapter(Context context, IReadOnlyList<T> data) {
            _context = context;
            _data = data;
        }

        public IReadOnlyList<T> Data {
            get {
                return _data;
            }
            set {
                if (value != _data) {
                    _data = value;

                    if (value == null) {
                        this.NotifyDataSetInvalidated();
                    }
                    else {
                        this.NotifyDataSetChanged();
                    }
                }
            }
        }

        protected Context Context {
            get {
                return _context;
            }
        }

        #region implemented abstract members of BaseAdapter

        public override long GetItemId(int position) {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent) {
            if (convertView == null) {
                convertView = CreateView(position, parent, (LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService));
            }

            if (Count > 0 && position < Count) {
                UpdateView(position, convertView, _data[position]);
            }

            return convertView;
        }

        public override int Count {
            get {
                if (_data == null)
                    return 0;
                else
                    return _data.Count;
            }
        }

        public override T this[int index] {
            get {
                return _data[index];
            }
        }

        #endregion

        protected abstract View CreateView(int position, ViewGroup parent, LayoutInflater inflater);

        protected abstract void UpdateView(int position, View view, T data);

    }

}

