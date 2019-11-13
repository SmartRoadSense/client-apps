using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmartRoadSense
{
    public class SessionManager : ISessionManager, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        static readonly Lazy<SessionManager> _instance = new Lazy<SessionManager> (() => new SessionManager());

        public static SessionManager Instance => _instance.Value;

	}
}
