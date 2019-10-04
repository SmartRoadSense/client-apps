using System;
namespace SmartRoadSense
{
    public class DataInteractor : IDataInteractor
    {
        IDataOutputActionPresenter _presenter;

        public DataInteractor(IDataOutputActionPresenter presenter)
        {
            _presenter = presenter;
        }
    }
}
