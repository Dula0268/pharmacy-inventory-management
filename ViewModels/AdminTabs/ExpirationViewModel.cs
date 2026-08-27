using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PharmacyInventory.Commands;
using PharmacyInventory.Services;

namespace PharmacyInventory.ViewModels.AdminTabs
{
    public class ExpirationViewModel : BaseViewModel
    {
        private readonly IProductService _productService;

        private int _selectedMonths = 1;
        public int SelectedMonths
        {
            get => _selectedMonths;
            set
            {
                if (_selectedMonths != value)
                {
                    _selectedMonths = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ExpirationRow> Results { get; } = new();

        public AsyncRelayCommand RefreshCommand { get; }

        public ExpirationViewModel(IProductService productService)
        {
            _productService = productService;

            RefreshCommand = new AsyncRelayCommand(LoadAsync);

            // Auto-load once when tab opens
            _ = LoadAsync();
        }

        public async Task LoadAsync()
        {
            try
            {
                var all = await _productService.GetAllProductsAsync();

                var today = DateOnly.FromDateTime(DateTime.Today);
                var max = today.AddMonths(SelectedMonths);

                // Show: Expired OR expiring within selected months
                var items = all
                    .Select(p => new
                    {
                        Product = p,
                        Expiry = p.Type == PharmacyInventory.Models.ProductType.Grocery ? p.ExdDate : p.ExpDate
                    })
                    .Where(x => x.Expiry.HasValue && (x.Expiry.Value < today || x.Expiry.Value <= max))
                    .OrderBy(x => x.Expiry)
                    .Select(p => new ExpirationRow
                    {
                        Type = p.Product.Type,
                        DisplayName = p.Product.DisplayName,
                        QuantityOnHand = p.Product.QuantityOnHand,
                        ExpDate = p.Expiry,
                        Status = (p.Expiry!.Value < today) ? "Expired" : "Near expiry"
                    })
                    .ToList();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Results.Clear();
                    foreach (var row in items)
                        Results.Add(row);
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                    MessageBox.Show(ex.ToString(), "Expiration Error", MessageBoxButton.OK, MessageBoxImage.Error));
            }
        }
    }
}
