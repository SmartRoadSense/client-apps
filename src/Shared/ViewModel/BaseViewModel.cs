using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace SmartRoadSense.Shared.ViewModel {

    public abstract class BaseViewModel: INotifyPropertyChanged {

        protected BaseViewModel() {
        }

        /// <summary>
        /// Called by controller when first created.
        /// </summary>
        public virtual void OnCreate() {
        }

        /// <summary>
        /// Called by controller when destroyed.
        /// </summary>
        public virtual void OnDestroy() {
        }

        #region INotifyPropertyChanged implementation

        /// <summary>
        /// Occurs when a property changes its value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">Weakly typed property name.</param>
        protected virtual void OnPropertyChanged(string propertyName) {
            var evt = PropertyChanged;
            if (evt != null) {
                evt(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Raises the property changed event on a strongly typed property.
        /// </summary>
        /// <param name="property">Strongly typed property expression.</param>
        protected void OnPropertyChanged<TProperty>(Expression<Func<TProperty>> property) {
            OnPropertyChanged(property.GetMemberInfo().Name);
        }

        #endregion

    }

}

