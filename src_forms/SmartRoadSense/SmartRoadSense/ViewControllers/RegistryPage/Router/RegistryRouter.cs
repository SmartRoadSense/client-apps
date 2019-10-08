using System;
namespace SmartRoadSense
{
    public class RegistryRouter : IRegistryRouter
    {
        IRegistryView _view;
        MainMasterDetailPage _master;

        public RegistryRouter(IRegistryView view, MainMasterDetailPage master)
        {
            _view = view;
            _master = master;
        }
    }
}
