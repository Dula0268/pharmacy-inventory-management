using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PharmacyInventory.Models;

namespace PharmacyInventory.Services
{
    public interface IReportService
    {
        Task<DailyReportResult> GetDailyReportAsync(DateOnly date);
        Task<MonthlyReportResult> GetMonthlyReportAsync(int year, int month);
    }

    public class DailyReportResult
    {
        public decimal TotalAmount { get; set; }
        public List<SaleSummary> Sales { get; set; } = new();
    }

    public class SaleSummary
    {
        public long SaleId { get; set; }
        public DateTime SoldAt { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class MonthlyReportResult
    {
        public decimal TotalAmount { get; set; }
        public List<DailyTotal> DailyTotals { get; set; } = new();
        public List<TopProduct> TopProducts { get; set; } = new();
    }

    public class DailyTotal
    {
        public int Day { get; set; }
        public decimal Total { get; set; }
    }

    public class TopProduct
    {
        public int ProductId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public int TotalQty { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
