using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmartRoadSense
{
    public class BaseViewBinder : INotifyPropertyChanged
    {
        /* Notify Property Changed Interface */
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /* Properties Helper */
        Dictionary<string, object> _properties = new Dictionary<string, object>();

        protected void SetTempValue<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (!_properties.ContainsKey(propertyName))
            {
                _properties.Add(propertyName, default(T));
            }

            var oldValue = GetTempValue<T>(propertyName);
            if (!EqualityComparer<T>.Default.Equals(oldValue, value))
            {
                _properties[propertyName] = value;
                OnPropertyChanged(propertyName);
            }
        }

        protected T GetTempValue<T>([CallerMemberName] string propertyName = null)
        {
            if (!_properties.ContainsKey(propertyName))
            {
                return default(T);
            }
            else
            {
                return (T)_properties[propertyName];
            }
        }
    }
}