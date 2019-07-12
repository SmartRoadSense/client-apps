using System.Threading.Tasks;

namespace SmartRoadSense
{
    public interface IMainMenuRouter
    {
        Task<bool> GoToHomePage();
    }
}
