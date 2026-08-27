using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmacyInventory.Data;
using PharmacyInventory.Models;

namespace PharmacyInventory.Services
{
    public class ReportService : IReportService
    {
        private readonly PharmacyDbContext _db;

        public ReportService(PharmacyDbContext db)
        {
            _db = db;
        }

        public async Task<DailyReportResult> GetDailyReportAsync(DateOnly date)
        {
            var start = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var end = start.AddDays(1);

            var salesQuery = _db.Sales
                .AsNoTracking()
                .Where(s => s.SoldAt >= start && s.SoldAt < end);

            // ✅ SQLite-safe sum (sum as double)
            var totalDouble = await salesQuery
                .Select(s => (double?)s.TotalAmount)
                .SumAsync() ?? 0.0;

            var sales = await salesQuery
                .Select(s => new SaleSummary
                {
                    SaleId = s.Id,
                    SoldAt = s.SoldAt,
                    TotalAmount = s.TotalAmount
                })
                .OrderBy(s => s.SoldAt)
                .ToListAsync();

            return new DailyReportResult
            {
                TotalAmount = (decimal)totalDouble,
                Sales = sales
            };
        }

        public async Task<MonthlyReportResult> GetMonthlyReportAsync(int year, int month)
        {
            var start = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = start.AddMonths(1);

            var salesQuery = _db.Sales
                .AsNoTracking()
                .Where(s => s.SoldAt >= start && s.SoldAt < end);

            // ✅ SQLite-safe sum (sum as double)
            var totalDouble = await salesQuery
                .Select(s => (double?)s.TotalAmount)
                .SumAsync() ?? 0.0;

            // ✅ daily totals (sum as double)
            var dailyTotals = await salesQuery
                .GroupBy(s => s.SoldAt.Date)
                .Select(g => new DailyTotal
                {
                    Day = g.Key.Day,
                    Total = (decimal)(g.Select(x => (double?)x.TotalAmount).Sum() ?? 0.0)
                })
                .OrderBy(d => d.Day)
                .ToListAsync();

            // ✅ Top products: do NOT group by DisplayName in SQL (DisplayName is computed)
            // Filter sale items by sale date using navigation
            var topRaw = await _db.SaleItems
                .AsNoTracking()
                .Where(si => si.Sale.SoldAt >= start && si.Sale.SoldAt < end)
                .GroupBy(si => si.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQty = g.Sum(x => x.Qty),
                    TotalAmount = g.Select(x => (double?)x.LineTotal).Sum() ?? 0.0
                })
                .OrderByDescending(x => x.TotalQty)
                .ThenByDescending(x => x.TotalAmount)
                .ToListAsync();

            var ids = topRaw.Select(x => x.ProductId).Distinct().ToList();

            var products = await _db.Products
                .AsNoTracking()
                .Where(p => ids.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            var topProducts = topRaw
                .Select(x =>
                {
                    products.TryGetValue(x.ProductId, out var p);
                    return new TopProduct
                    {
                        ProductId = x.ProductId,
                        DisplayName = p?.DisplayName ?? $"Product #{x.ProductId}",
                        TotalQty = x.TotalQty,
                        TotalAmount = (decimal)x.TotalAmount
                    };
                })
                .ToList();

            return new MonthlyReportResult
            {
                TotalAmount = (decimal)totalDouble,
                DailyTotals = dailyTotals,
                TopProducts = topProducts
            };
        }
    }
}
