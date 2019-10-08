using System;
namespace SmartRoadSense
{
    public class RegistryPresenter : IRegistryDataPresenter, IRegistryInputActionPresenter, IRegistryOutputActionPresenter
    {
        IRegistryRouter _router;
        IRegistryInteractor _interactor;

        public RegistryPresenter(IRegistryView view, MainMasterDetailPage master)
        {
            _router = new RegistryRouter(view, master);
            _interactor = new RegistryInteractor(this);
        }
    }
}
