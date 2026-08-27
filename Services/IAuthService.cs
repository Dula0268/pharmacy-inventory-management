using System.Threading.Tasks;
using PharmacyInventory.Models;

namespace PharmacyInventory.Services
{
    public interface IAuthService
    {
        Task<AppUser?> LoginAsync(string username, string password);
    }
}
