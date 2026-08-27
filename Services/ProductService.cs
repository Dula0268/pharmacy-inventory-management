using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmacyInventory.Data;
using PharmacyInventory.Helpers;
using PharmacyInventory.Models;

namespace PharmacyInventory.Services
{
    public class ProductService : IProductService
    {
        private readonly PharmacyDbContext _db;

        public ProductService(PharmacyDbContext db)
        {
            _db = db;
        }

        public async Task<Product> AddMedicineAsync(Product medicine)
        {
            if (medicine is null) throw new ArgumentNullException(nameof(medicine));

            if (string.IsNullOrWhiteSpace(medicine.Category))
                throw new ArgumentException("Category is required for medicine.", nameof(medicine));

            if (string.IsNullOrWhiteSpace(medicine.GenericName))
                throw new ArgumentException("GenericName is required for medicine.", nameof(medicine));

            if (string.IsNullOrWhiteSpace(medicine.BrandName))
                throw new ArgumentException("BrandName is required for medicine.", nameof(medicine));

            if (string.IsNullOrWhiteSpace(medicine.BatchNo))
                throw new ArgumentException("BatchNo is required for medicine.", nameof(medicine));

            if (medicine.UnitPrice < 0 || medicine.PackPrice < 0 || medicine.TotalValue < 0)
                throw new ArgumentException("Medicine prices must be non-negative.", nameof(medicine));

            if (medicine.NoOfUnits < 0 || medicine.UnitsPerPack < 0 || medicine.NoOfPacks < 0)
                throw new ArgumentException("Medicine quantities must be >= 0.", nameof(medicine));

            medicine.Type = ProductType.Medicine;
            medicine.QuantityOnHand = medicine.NoOfUnits > 0 && medicine.NoOfPacks > 0
                ? medicine.NoOfUnits * medicine.NoOfPacks
                : medicine.NoOfUnits;
            medicine.SellingPrice = medicine.UnitPrice;
            medicine.BuyingPrice = medicine.PackPrice;
            _db.Products.Add(medicine);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return medicine;
        }

        public async Task<Product> AddGroceryAsync(Product grocery)
        {
            if (grocery is null) throw new ArgumentNullException(nameof(grocery));

            if (string.IsNullOrWhiteSpace(grocery.ItemType))
                throw new ArgumentException("ItemType is required for grocery.", nameof(grocery));

            if (string.IsNullOrWhiteSpace(grocery.Brand))
                throw new ArgumentException("Brand is required for grocery.", nameof(grocery));

            if (string.IsNullOrWhiteSpace(grocery.Size))
                throw new ArgumentException("Size is required for grocery.", nameof(grocery));

            if (grocery.Price < 0 || grocery.Total < 0)
                throw new ArgumentException("Grocery prices must be non-negative.", nameof(grocery));

            if (grocery.Count < 0)
                throw new ArgumentException("Count must be >= 0.", nameof(grocery));

            grocery.Type = ProductType.Grocery;
            grocery.QuantityOnHand = grocery.Count;
            grocery.SellingPrice = grocery.Price;
            grocery.BuyingPrice = grocery.Total;
            _db.Products.Add(grocery);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return grocery;
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string text, ProductType? typeFilter = null)
        {
            text ??= string.Empty;
            var q = _db.Products.AsNoTracking().AsQueryable();

            if (typeFilter.HasValue)
                q = q.Where(p => p.Type == typeFilter.Value);

            text = text.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                var t = text.ToLowerInvariant();
                q = q.Where(p => (p.Type == ProductType.Medicine && (
                            (p.Category ?? string.Empty).ToLower().Contains(t) ||
                            (p.BrandName ?? string.Empty).ToLower().Contains(t) ||
                            (p.GenericName ?? string.Empty).ToLower().Contains(t) ||
                            (p.BatchNo ?? string.Empty).ToLower().Contains(t) ||
                            (p.Packing ?? string.Empty).ToLower().Contains(t)
                        ))
                        || (p.Type == ProductType.Grocery && (
                            (p.ItemType ?? string.Empty).ToLower().Contains(t) ||
                            (p.Brand ?? string.Empty).ToLower().Contains(t) ||
                            (p.Speciality ?? string.Empty).ToLower().Contains(t) ||
                            (p.Size ?? string.Empty).ToLower().Contains(t) ||
                            (p.Note ?? string.Empty).ToLower().Contains(t)
                        )));
            }

            var list = await q.ToListAsync().ConfigureAwait(false);
            return list.OrderBy(p => p.Type).ThenBy(p => p.DisplayName).ToList();
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            var list = await _db.Products.AsNoTracking().ToListAsync().ConfigureAwait(false);
            return list.OrderBy(p => p.Type).ThenBy(p => p.DisplayName).ToList();
        }

        public async Task UpdateProductAsync(Product product)
        {
            if (product is null) throw new ArgumentNullException(nameof(product));

            var existing = await _db.Products.FindAsync(product.Id).ConfigureAwait(false);
            if (existing is null) throw new InvalidOperationException("Product not found.");

            // Update allowed fields
            existing.Type = product.Type;
            existing.Category = product.Category;
            existing.GenericName = product.GenericName;
            existing.BrandName = product.BrandName;
            existing.Strength = product.Strength;
            existing.DosageForm = product.DosageForm;
            existing.BatchNo = product.BatchNo;
            existing.ExpDate = product.ExpDate;
            existing.MfdDate = product.MfdDate;
            existing.Packing = product.Packing;
            existing.NoOfUnits = product.NoOfUnits;
            existing.UnitPrice = product.UnitPrice;
            existing.UnitsPerPack = product.UnitsPerPack;
            existing.NoOfPacks = product.NoOfPacks;
            existing.PackPrice = product.PackPrice;
            existing.TotalValue = product.TotalValue;

            existing.ItemType = product.ItemType;
            existing.Brand = product.Brand;
            existing.Speciality = product.Speciality;
            existing.Size = product.Size;
            existing.Price = product.Price;
            existing.Count = product.Count;
            existing.Total = product.Total;
            existing.ExdDate = product.ExdDate;
            existing.OutColour = product.OutColour;
            existing.Note = product.Note;

            existing.QuantityOnHand = product.QuantityOnHand;
            existing.BuyingPrice = product.BuyingPrice;
            existing.SellingPrice = product.SellingPrice;
            existing.GroceryName = product.GroceryName;

            existing.PackSize = product.PackSize;
            existing.PricePerQuantity = product.PricePerQuantity;
            existing.Distributor = product.Distributor;

            if (existing.Type == ProductType.Medicine)
            {
                existing.QuantityOnHand = existing.NoOfUnits > 0 && existing.NoOfPacks > 0
                    ? existing.NoOfUnits * existing.NoOfPacks
                    : existing.NoOfUnits;
                existing.SellingPrice = existing.UnitPrice;
                existing.BuyingPrice = existing.PackPrice;
            }
            else if (existing.Type == ProductType.Grocery)
            {
                existing.QuantityOnHand = existing.Count;
                existing.SellingPrice = existing.Price;
                existing.BuyingPrice = existing.Total;
            }

            await _db.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
