using System.Collections.Generic;
using System.Threading.Tasks;
using PharmacyInventory.Models;

namespace PharmacyInventory.Services
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryItem>> GetAllAsync();
        Task<InventoryItem?> GetByIdAsync(System.Guid id);
        Task AddAsync(InventoryItem item);
        Task UpdateAsync(InventoryItem item);
        Task DeleteAsync(System.Guid id);
    }
}
