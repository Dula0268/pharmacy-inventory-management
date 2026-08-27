using System.Collections.Generic;
using System.Threading.Tasks;
using PharmacyInventory.Models;

namespace PharmacyInventory.Services
{
    public interface IProductService
    {
        Task<Product> AddMedicineAsync(Product medicine);
        Task<Product> AddGroceryAsync(Product grocery);
        Task<IEnumerable<Product>> SearchProductsAsync(string text, ProductType? typeFilter = null);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task UpdateProductAsync(Product product);
    }
}
