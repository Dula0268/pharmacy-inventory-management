using System;
using System.Linq;
using System.Threading.Tasks;
using PharmacyInventory.Services;
using PharmacyInventory.Models;

namespace PharmacyInventory.ViewModels.AdminTabs
{
    public class DashboardViewModel : ViewModels.BaseViewModel
    {
        private readonly IReportService _reportService;
        private readonly IProductService _productService;

        public decimal TodayTotal
        {
            get => _todayTotal;
            private set => SetProperty(ref _todayTotal, value);
        }
        private decimal _todayTotal;

        public int ExpiredCount
        {
            get => _expiredCount;
            private set => SetProperty(ref _expiredCount, value);
        }
        private int _expiredCount;

        public int NearExpiryCount
        {
            get => _nearExpiryCount;
            private set => SetProperty(ref _nearExpiryCount, value);
        }
        private int _nearExpiryCount;

        public int OutOfStockCount
        {
            get => _outOfStockCount;
            private set => SetProperty(ref _outOfStockCount, value);
        }
        private int _outOfStockCount;

        public DashboardViewModel(IReportService reportService, IProductService productService)
        {
            _reportService = reportService;
            _productService = productService;

            // fire-and-forget load (safe for startup); UI updates via property notifications
            _ = LoadAsync();
        }

        public async Task LoadAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            try
            {
                var daily = await _reportService.GetDailyReportAsync(today);
                TodayTotal = daily?.TotalAmount ?? 0m;
            }
            catch
            {
                TodayTotal = 0m;
            }

            try
            {
                var products = (await _productService.GetAllProductsAsync()).ToList();

                ExpiredCount = products.Count(p => p.ExpDate.HasValue && p.ExpDate.Value < today);
                NearExpiryCount = products.Count(p => p.ExpDate.HasValue && (p.ExpDate.Value >= today) && (p.ExpDate.Value <= today.AddDays(30)));
                OutOfStockCount = products.Count(p => p.QuantityOnHand <= 0);
            }
            catch
            {
                ExpiredCount = NearExpiryCount = OutOfStockCount = 0;
            }
        }
    }
}
