using System.Collections.Generic;
using System.Threading.Tasks;

namespace PharmacyInventory.Services
{
    public interface ISalesService
    {
        Task<long> SellAsync(int cashierUserId, List<(int productId, int qty)> cart);
    }
}
