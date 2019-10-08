using System;
using System.Collections.ObjectModel;

namespace SmartRoadSense
{
    public class RegistryViewBinder : BaseViewBinder, IRegistryView
    {
        IRegistryInputActionPresenter _presenter;
        IRegistryDataPresenter _dataPresenter;

        public RegistryViewBinder(RegistryPage page, MainMasterDetailPage master)
        {
            CurrentPage = page;

            var presenter = new RegistryPresenter(this, master);
            _presenter = presenter;
            _dataPresenter = presenter;

            RegistryListItems = new ObservableCollection<string>();
            for (var i = 0; i < 20; i++)
                RegistryListItems.Add("item " + i);

            _registryListItems = RegistryListItems;
        }

        public RegistryPage CurrentPage { get; }

        // BINDINGS
        public ObservableCollection<string> _registryListItems = new ObservableCollection<string>();

        public ObservableCollection<string> RegistryListItems
        {
            get => _registryListItems;
            set
            {
                _registryListItems = value;
                OnPropertyChanged();
            }
        }
    }

    
}
