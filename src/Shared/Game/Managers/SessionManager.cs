using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmartRoadSense.Shared {
    public class SessionManager : ISessionManager, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static SessionManager Instance { get; } = new SessionManager();
    }
}
