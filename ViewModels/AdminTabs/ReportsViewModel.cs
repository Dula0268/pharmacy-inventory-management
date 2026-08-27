using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PharmacyInventory.Commands;
using PharmacyInventory.Services;

namespace PharmacyInventory.ViewModels.AdminTabs
{
    public class ReportsViewModel : ViewModels.BaseViewModel
    {
        private readonly IReportService _reportService;

        public ReportsViewModel(IReportService reportService)
        {
            _reportService = reportService;
            DailySales = new ObservableCollection<DailySaleRow>();
            DailyTotals = new ObservableCollection<DailyTotalRow>();
            TopProducts = new ObservableCollection<TopProductRow>();

            SelectedDate = DateTime.UtcNow.Date;
            SelectedYear = DateTime.UtcNow.Year;
            SelectedMonth = DateTime.UtcNow.Month;

            LoadDailyCommand = new AsyncRelayCommand(async _ => await LoadDailyAsync());
            LoadMonthlyCommand = new AsyncRelayCommand(async _ => await LoadMonthlyAsync());

            // subscribe to sales changes so reports refresh automatically when sales occur
            PharmacyInventory.Helpers.AppEvents.SalesChanged += async (s, e) =>
            {
                try
                {
                    await Application.Current.Dispatcher.InvokeAsync(async () =>
                    {
                        await LoadDailyAsync();
                        await LoadMonthlyAsync();
                    });
                }
                catch
                {
                    // ignore; reports will refresh on next manual request
                }
            };
        }

        // Daily report
        public DateTime SelectedDate { get; set; }
        public ObservableCollection<DailySaleRow> DailySales { get; }
        public decimal DailyTotal { get; private set; }
        public AsyncRelayCommand LoadDailyCommand { get; }

        // Monthly report
        public int SelectedYear { get; set; }
        public int SelectedMonth { get; set; }
        public ObservableCollection<DailyTotalRow> DailyTotals { get; }
        public ObservableCollection<TopProductRow> TopProducts { get; }
        public decimal MonthlyTotal { get; private set; }
        public AsyncRelayCommand LoadMonthlyCommand { get; }

        public class DailySaleRow
        {
            public long SaleId { get; set; }
            public DateTime Time { get; set; }
            public decimal Total { get; set; }
        }

        public class DailyTotalRow
        {
            public int Day { get; set; }
            public decimal Total { get; set; }
        }

        public class TopProductRow
        {
            public string DisplayName { get; set; } = string.Empty;
            public int TotalQty { get; set; }
            public decimal TotalAmount { get; set; }
        }

        public async Task LoadDailyAsync()
        {
            try
            {
                var dto = await _reportService.GetDailyReportAsync(DateOnly.FromDateTime(SelectedDate));
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    DailySales.Clear();
                    foreach (var s in dto.Sales.OrderBy(s => s.SoldAt)) DailySales.Add(new DailySaleRow { SaleId = s.SaleId, Time = s.SoldAt, Total = s.TotalAmount });
                    DailyTotal = dto.TotalAmount;
                    OnPropertyChanged(nameof(DailyTotal));
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show(ex.Message, "Daily Report Error", MessageBoxButton.OK, MessageBoxImage.Error));
            }
        }

        public async Task LoadMonthlyAsync()
        {
            try
            {
                var dto = await _reportService.GetMonthlyReportAsync(SelectedYear, SelectedMonth);
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    DailyTotals.Clear();
                    foreach (var d in dto.DailyTotals.OrderBy(d => d.Day)) DailyTotals.Add(new DailyTotalRow { Day = d.Day, Total = d.Total });

                    TopProducts.Clear();
                    foreach (var t in dto.TopProducts.OrderByDescending(t => t.TotalQty)) TopProducts.Add(new TopProductRow { DisplayName = t.DisplayName, TotalQty = t.TotalQty, TotalAmount = t.TotalAmount });

                    MonthlyTotal = dto.TotalAmount;
                    OnPropertyChanged(nameof(MonthlyTotal));
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show(ex.Message, "Monthly Report Error", MessageBoxButton.OK, MessageBoxImage.Error));
            }
        }
    }
}
 
