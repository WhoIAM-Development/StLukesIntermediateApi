using System.Threading.Tasks;

namespace IntermediateAPI.Services
{
    public interface IAuthProvider
    {
        Task<string> AcquireTokenAsync();
    }
}