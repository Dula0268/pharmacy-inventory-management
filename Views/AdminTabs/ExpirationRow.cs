using System;
using PharmacyInventory.Models;

namespace PharmacyInventory.ViewModels.AdminTabs
{
    public class ExpirationRow
    {
        public ProductType Type { get; set; }
        public string DisplayName { get; set; } = "";
        public int QuantityOnHand { get; set; }
        public DateOnly? ExpDate { get; set; }
        public string Status { get; set; } = ""; // "Expired" or "Near expiry"
    }
}
