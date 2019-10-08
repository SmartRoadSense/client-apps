using System;
namespace SmartRoadSense
{
    public class RegistryInteractor : IRegistryInteractor
    {
        IRegistryOutputActionPresenter _presenter;

        public RegistryInteractor(IRegistryOutputActionPresenter presenter)
        {
            _presenter = presenter;
        }
    }
}
