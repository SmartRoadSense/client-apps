using System;
using System.Threading.Tasks;

namespace SmartRoadSense
{
    public interface IHomePageRouter
    {
        Task<bool> OpenCarpoolingPopup();
    }
}
