using System;
using System.Windows.Input;

namespace SmartRoadSense.Shared {

    /// <summary>
    /// Simple command that relays command action to a delegate.
    /// </summary>
    public class RelayCommand<T> : ICommand {

        public RelayCommand(Action<T> action) {
            _action = action;
        }

        private readonly Action<T> _action;

        public bool CanExecute(object parameter) {
            return (_action != null);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter) {
            if(_action != null && (parameter == null || parameter is T)) {
                _action((T)parameter);
            }
        }

    }

    /// <summary>
    /// Simple command that relays command action to a delegate.
    /// </summary>
    public class RelayCommand : ICommand {

        public RelayCommand(Action action) {
            _action = action;
        }

        private readonly Action _action;

        public bool CanExecute(object parameter) {
            return (_action != null);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter) {
            if(_action != null) {
                _action();
            }
        }

    }

}

