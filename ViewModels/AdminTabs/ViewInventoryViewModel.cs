using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PharmacyInventory.Commands;
using PharmacyInventory.Models;
using PharmacyInventory.Services;

namespace PharmacyInventory.ViewModels.AdminTabs
{
    public class ViewInventoryViewModel : ViewModels.BaseViewModel
    {
        private readonly IProductService _productService;

        public ViewInventoryViewModel(IProductService productService)
        {
            _productService = productService;
            Products = new ObservableCollection<ProductRow>();
            RefreshCommand = new AsyncRelayCommand(async _ => await LoadAsync());
        }

        public ObservableCollection<ProductRow> Products { get; }

        public AsyncRelayCommand RefreshCommand { get; }

        public async Task LoadAsync()
        {
            try
            {
                var all = await _productService.GetAllProductsAsync();
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Products.Clear();
                    foreach (var p in all.OrderBy(p => p.Type).ThenBy(p => p.DisplayName))
                    {
                        Products.Add(new ProductRow(p));
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show(ex.Message, "Inventory Error", MessageBoxButton.OK, MessageBoxImage.Error));
            }
        }

        public class ProductRow
        {
            private readonly Product _p;
            public ProductRow(Product p) { _p = p; }

            public ProductType Type => _p.Type;
            public string DisplayName => _p.DisplayName ?? string.Empty;
            public int QuantityOnHand => _p.QuantityOnHand;
            public decimal BuyingPrice => _p.BuyingPrice;
            public decimal SellingPrice => _p.SellingPrice;
            public DateOnly? MfdDate => _p.MfdDate;
            public DateOnly? ExpDate => _p.Type == ProductType.Grocery ? _p.ExdDate : _p.ExpDate;
            public string? Distributor => _p.Distributor;

            public string Status
            {
                get
                {
                    var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);
                    if (_p.QuantityOnHand <= 0) return "Out of stock";
                    var expiry = _p.Type == ProductType.Grocery ? _p.ExdDate : _p.ExpDate;
                    if (expiry.HasValue && expiry.Value < today) return "Expired";
                    return "Available";
                }
            }
        }
    }
}
