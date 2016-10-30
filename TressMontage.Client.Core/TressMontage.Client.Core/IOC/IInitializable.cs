using System.Threading.Tasks;

namespace TressMontage.Core.IOC
{
    public interface IInitializable
    {
        Task Initialize();
    }
}