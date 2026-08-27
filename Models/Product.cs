using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyInventory.Models
{
    public class Product
    {
        public int Id { get; set; }

        public ProductType Type { get; set; }

        // Medicine fields
        public string? Category { get; set; }
        public string? GenericName { get; set; }
        public string? BrandName { get; set; }
        public string? Strength { get; set; }
        public string? DosageForm { get; set; }
        public string? BatchNo { get; set; }
        public DateOnly? ExpDate { get; set; }
        public DateOnly? MfdDate { get; set; }
        public string? Packing { get; set; }
        public int NoOfUnits { get; set; }
        public decimal UnitPrice { get; set; }
        public int UnitsPerPack { get; set; }
        public int NoOfPacks { get; set; }
        public decimal PackPrice { get; set; }
        public decimal TotalValue { get; set; }

        // Grocery fields
        public string? ItemType { get; set; }
        public string? Brand { get; set; }
        public string? Speciality { get; set; }
        public string? Size { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
        public decimal Total { get; set; }
        public DateOnly? ExdDate { get; set; }
        public string? OutColour { get; set; }
        public string? Note { get; set; }

        public int QuantityOnHand { get; set; }

        public decimal BuyingPrice { get; set; }
        public decimal SellingPrice { get; set; }

        // Legacy fields kept for compatibility with older data and views.
        public string? GroceryName { get; set; }
        public string? PackSize { get; set; }
        public decimal? PricePerQuantity { get; set; }
        public string? Distributor { get; set; }

        // Navigation
        public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

        [NotMapped]
        public string DisplayName
        {
            get
            {
                if (Type == ProductType.Medicine)
                {
                    var medicineParts = new[] { Category, BrandName, GenericName }
                        .Where(part => !string.IsNullOrWhiteSpace(part))
                        .Select(part => part!.Trim())
                        .ToArray();

                    if (medicineParts.Length > 0)
                        return string.Join(" - ", medicineParts);

                    if (!string.IsNullOrWhiteSpace(PackSize))
                        return PackSize.Trim();

                    return string.Empty;
                }

                var groceryParts = new[] { ItemType, Brand, Speciality, Size }
                    .Where(part => !string.IsNullOrWhiteSpace(part))
                    .Select(part => part!.Trim())
                    .ToArray();

                if (groceryParts.Length > 0)
                    return string.Join(" - ", groceryParts);

                return GroceryName ?? string.Empty;
            }
        }
    }
}
