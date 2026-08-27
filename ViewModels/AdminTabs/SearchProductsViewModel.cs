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
    public class SearchProductsViewModel : ViewModels.BaseViewModel
    {
        private readonly IProductService _productService;

        public SearchProductsViewModel(IProductService productService)
        {
            _productService = productService;
            Results = new ObservableCollection<Product>();
            SelectedFilter = "All";
            SearchCommand = new AsyncRelayCommand(async _ => await SearchAsync());
        }

        public string SearchText { get; set; } = string.Empty;
        public string SelectedFilter { get; set; }

        public ObservableCollection<Product> Results { get; }

        public AsyncRelayCommand SearchCommand { get; }

        private ProductType? ResolveFilter()
        {
            if (string.Equals(SelectedFilter, "Medicine", StringComparison.OrdinalIgnoreCase)) return ProductType.Medicine;
            if (string.Equals(SelectedFilter, "Grocery", StringComparison.OrdinalIgnoreCase)) return ProductType.Grocery;
            return null;
        }

        private async Task SearchAsync()
        {
            try
            {
                var type = ResolveFilter();
                var items = await _productService.SearchProductsAsync(SearchText ?? string.Empty, type);
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Results.Clear();
                    foreach (var p in items.OrderBy(p => p.Type).ThenBy(p => p.DisplayName)) Results.Add(p);
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show(ex.Message, "Search Error", MessageBoxButton.OK, MessageBoxImage.Error));
            }
        }
    }
}
 
