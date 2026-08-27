using System;
using System.Collections.Generic;

namespace PharmacyInventory.Models
{
    public class Sale
    {
        public long Id { get; set; }
        public DateTime SoldAt { get; set; } = DateTime.UtcNow;

        public int CashierUserId { get; set; }
        public AppUser? CashierUser { get; set; }

        public decimal TotalAmount { get; set; }

        public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
    }
}
