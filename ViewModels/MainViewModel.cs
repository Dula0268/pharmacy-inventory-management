using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PharmacyInventory.Models;
using PharmacyInventory.Services;
using PharmacyInventory.Commands;

namespace PharmacyInventory.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IInventoryService _inventoryService;

        public ObservableCollection<InventoryItem> Items { get; } = new();

        public RelayCommand RefreshCommand { get; }
        public AsyncRelayCommand LoadCommand { get; }

        public MainViewModel(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
            RefreshCommand = new RelayCommand(async () => await LoadAsync());
            LoadCommand = new AsyncRelayCommand(async () => await LoadAsync());
        }

        public async Task LoadAsync()
        {
            Items.Clear();
            var items = await _inventoryService.GetAllAsync();
            foreach (var it in items)
                Items.Add(it);
        }
    }
}
