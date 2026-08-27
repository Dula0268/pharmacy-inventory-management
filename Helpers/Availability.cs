using System;
using PharmacyInventory.Models;

namespace PharmacyInventory.Helpers
{
    public static class Availability
    {
        public static bool IsExpired(Product p, DateOnly today)
        {
            if (p is null) throw new ArgumentNullException(nameof(p));
            var expiry = p.Type == ProductType.Grocery ? p.ExdDate : p.ExpDate;
            return expiry.HasValue && expiry.Value < today;
        }

        public static string Status(Product p, DateOnly today)
        {
            if (p is null) throw new ArgumentNullException(nameof(p));

            if (IsExpired(p, today))
                return "Expired";

            if (p.QuantityOnHand <= 0)
                return "Out of stock";

            return "Available";
        }

        // Cashier may view all products but may only add to cart when status == "Available"
        public static bool CanAddToCartForCashier(Product p, DateOnly today)
        {
            if (p is null) throw new ArgumentNullException(nameof(p));
            return Status(p, today) == "Available";
        }
    }
}
