using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PharmacyInventory.Commands;
using PharmacyInventory.Helpers;
using PharmacyInventory.Models;
using PharmacyInventory.Services;
using System.Windows;

namespace PharmacyInventory.ViewModels
{
    public class CashierViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        private readonly ISalesService _salesService;

        public ObservableCollection<ProductRow> Products { get; } = new();
        public ObservableCollection<CartItem> Cart { get; } = new();

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        private ProductType? _filterType = null;
        public ProductType? FilterType
        {
            get => _filterType;
            set => SetProperty(ref _filterType, value);
        }

        // helper for ComboBox binding (All/Medicine/Grocery)
        private string? _filterSelection;
        public string? FilterSelection
        {
            get => _filterSelection;
            set
            {
                _filterSelection = value;
                if (string.IsNullOrEmpty(value)) FilterType = null;
                else if (value == "Medicine") FilterType = ProductType.Medicine;
                else if (value == "Grocery") FilterType = ProductType.Grocery;
                OnPropertyChanged(nameof(FilterSelection));
            }
        }

        public RelayCommand SearchCommand { get; }
        public RelayCommand AddToCartCommand { get; }
        public RelayCommand RemoveFromCartCommand { get; }
        public AsyncRelayCommand SellCommand { get; }
        public RelayCommand IncreaseQtyCommand { get; }
        public RelayCommand DecreaseQtyCommand { get; }

        public int CurrentCashierId { get; set; } = 2;

        public CashierViewModel(IProductService productService, ISalesService salesService)
        {
            _productService = productService;
            _salesService = salesService;

            SearchCommand = new RelayCommand(async _ => await LoadProductsAsync());
            AddToCartCommand = new RelayCommand(param => AddToCart(param as ProductRow), param => CanAddToCart(param as ProductRow));
            RemoveFromCartCommand = new RelayCommand(param => RemoveFromCart(param as CartItem));
            SellCommand = new AsyncRelayCommand(async _ => await ExecuteSellAsync(), _ => Cart.Count > 0);
            IncreaseQtyCommand = new RelayCommand(param => IncreaseQty(param as CartItem));
            DecreaseQtyCommand = new RelayCommand(param => DecreaseQty(param as CartItem));
        }

        public async Task LoadProductsAsync()
        {
            var list = await _productService.SearchProductsAsync(SearchText ?? string.Empty, FilterType);
            Products.Clear();
            foreach (var p in list)
            {
                Products.Add(new ProductRow(p));
            }
        }

        private bool CanAddToCart(ProductRow? row)
        {
            if (row is null) return false;
            return row.AvailabilityStatus == "Available";
        }

        private void AddToCart(ProductRow? row)
        {
            if (row is null) return;

            var existing = Cart.FirstOrDefault(c => c.ProductId == row.Product.Id);
            var available = row.AvailableQuantity;
            if (existing != null)
            {
                if (available <= 0) throw new InvalidOperationException("Not enough stock to add more.");
                existing.Qty += 1;
            }
            else
            {
                if (available <= 0) throw new InvalidOperationException("Not enough stock to add.");
                var item = new CartItem(row.Product.Id, row, 1, row.Product.SellingPrice);
                item.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(CartItem.Qty)) OnPropertyChanged(nameof(Total)); };
                Cart.Add(item);
            }

            row.ReservedQty += 1;
            OnPropertyChanged(nameof(Total));
            // update availability of the row
            row.RefreshAvailability();
            // update command states
            SellCommand.RaiseCanExecuteChanged();
            AddToCartCommand.RaiseCanExecuteChanged();
        }

        private void RemoveFromCart(CartItem? item)
        {
            if (item is null) return;
            var row = item.SourceRow;
            row.ReservedQty -= item.Qty;
            Cart.Remove(item);
            OnPropertyChanged(nameof(Total));
            row.RefreshAvailability();
            // update command states
            SellCommand.RaiseCanExecuteChanged();
            AddToCartCommand.RaiseCanExecuteChanged();
        }

        private void IncreaseQty(CartItem? item)
        {
            if (item is null) return;
            item.Qty += 1;
            OnPropertyChanged(nameof(Total));
            AddToCartCommand.RaiseCanExecuteChanged();
        }

        private void DecreaseQty(CartItem? item)
        {
            if (item is null) return;
            if (item.Qty <= 1) return;
            item.Qty -= 1;
            OnPropertyChanged(nameof(Total));
            AddToCartCommand.RaiseCanExecuteChanged();
        }

        public decimal Total => Cart.Sum(c => c.LineTotal);

        private async Task ExecuteSellAsync()
        {
            if (Cart.Count == 0) return;

            var cartList = Cart.Select(c => (c.ProductId, c.Qty)).ToList();

            try
            {
                var saleId = await _salesService.SellAsync(CurrentCashierId, cartList);

                // UI notification and refresh on UI thread
                await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show("Sale completed", "Success", MessageBoxButton.OK, MessageBoxImage.Information));

                Cart.Clear();
                await LoadProductsAsync();
                OnPropertyChanged(nameof(Total));
                // update command states
                SellCommand.RaiseCanExecuteChanged();
                AddToCartCommand.RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show(ex.Message, "Sale Error", MessageBoxButton.OK, MessageBoxImage.Error));
                throw;
            }
        }

        public class ProductRow : BaseViewModel
        {
            public Product Product { get; }
            public int ReservedQty { get; set; }

            public ProductRow(Product product)
            {
                Product = product;
            }

            public int AvailableQuantity => Product.QuantityOnHand - ReservedQty;

            public string AvailabilityStatus
            {
                get
                {
                    if (Availability.IsExpired(Product, DateOnly.FromDateTime(DateTime.UtcNow)))
                        return "Expired";
                    if (AvailableQuantity <= 0)
                        return "Out of stock";
                    return "Available";
                }
            }

            public void RefreshAvailability()
            {
                OnPropertyChanged(nameof(AvailableQuantity));
                OnPropertyChanged(nameof(AvailabilityStatus));
            }
        }

        public class CartItem : BaseViewModel
        {
            public int ProductId { get; }
            public ProductRow SourceRow { get; }

            public string DisplayName => SourceRow?.Product?.DisplayName ?? string.Empty;

            private int _qty;
            public int Qty
            {
                get => _qty;
                set
                {
                    if (value <= 0) throw new ArgumentException("Quantity must be >= 1");

                    var maxAllowed = SourceRow.Product.QuantityOnHand - (SourceRow.ReservedQty - _qty);
                    if (value > maxAllowed) throw new InvalidOperationException("Not enough stock for requested quantity.");

                    // adjust reserved qty on the source row
                    SourceRow.ReservedQty += (value - _qty);
                    SetProperty(ref _qty, value);
                    LineTotal = UnitPrice * _qty;
                    SourceRow.RefreshAvailability();
                }
            }

            private decimal _unitPrice;
            public decimal UnitPrice
            {
                get => _unitPrice;
                set => SetProperty(ref _unitPrice, value);
            }

            private decimal _lineTotal;
            public decimal LineTotal
            {
                get => _lineTotal;
                set => SetProperty(ref _lineTotal, value);
            }

            public CartItem(int productId, ProductRow sourceRow, int qty, decimal unitPrice)
            {
                ProductId = productId;
                SourceRow = sourceRow;
                _qty = qty;
                _unitPrice = unitPrice;
                _lineTotal = qty * unitPrice;
            }
        }
    }
}
