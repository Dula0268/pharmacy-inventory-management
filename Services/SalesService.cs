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
    public class SalesService : ISalesService
    {
        private readonly PharmacyDbContext _db;

        public SalesService(PharmacyDbContext db)
        {
            _db = db;
        }

        public async Task<long> SellAsync(int cashierUserId, List<(int productId, int qty)> cart)
        {
            if (cart is null || cart.Count == 0)
                throw new ArgumentException("Cart is empty.", nameof(cart));

            // Validate cashier exists and is active
            var cashier = await _db.Users.FindAsync(cashierUserId).ConfigureAwait(false);
            if (cashier is null || !cashier.IsActive)
                throw new InvalidOperationException("Invalid or inactive cashier.");

            var productIds = cart.Select(c => c.productId).Distinct().ToList();

            var products = await _db.Products.Where(p => productIds.Contains(p.Id)).ToListAsync().ConfigureAwait(false);

            // start transaction
            await using var tx = await _db.Database.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                decimal total = 0m;

                var sale = new Sale
                {
                    CashierUserId = cashierUserId,
                    SoldAt = DateTime.UtcNow,
                    TotalAmount = 0m,
                    Items = new List<SaleItem>()
                };

                foreach (var (productId, qty) in cart)
                {
                    if (qty <= 0)
                        throw new InvalidOperationException($"Invalid quantity for product {productId}.");

                    var product = products.FirstOrDefault(p => p.Id == productId);
                    if (product is null)
                        throw new InvalidOperationException($"Product not found: {productId}");

                    if (Availability.IsExpired(product, today))
                        throw new InvalidOperationException($"Expired product: {product.DisplayName}");

                    if (product.QuantityOnHand < qty)
                        throw new InvalidOperationException($"Not enough stock for {product.DisplayName}");

                    product.QuantityOnHand -= qty;

                    var unitPrice = product.SellingPrice;
                    var lineTotal = unitPrice * qty;
                    total += lineTotal;

                    var item = new SaleItem
                    {
                        ProductId = product.Id,
                        Qty = qty,
                        UnitPrice = unitPrice,
                        LineTotal = lineTotal
                    };

                    sale.Items.Add(item);
                }

                sale.TotalAmount = total;

                _db.Sales.Add(sale);
                await _db.SaveChangesAsync().ConfigureAwait(false);
                await tx.CommitAsync().ConfigureAwait(false);

                // Notify UI/reporting that sales/inventory changed
                try
                {
                    PharmacyInventory.Helpers.AppEvents.NotifySalesChanged();
                }
                catch
                {
                    // ignore notification errors
                }

                return sale.Id;
            }
            catch
            {
                try { await tx.RollbackAsync().ConfigureAwait(false); } catch { }
                throw;
            }
        }
    }
}
