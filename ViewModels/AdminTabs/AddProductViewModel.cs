using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using PharmacyInventory.Commands;
using PharmacyInventory.Models;
using PharmacyInventory.Services;

namespace PharmacyInventory.ViewModels.AdminTabs
{
    public class AddProductViewModel : ViewModels.BaseViewModel
    {
        private readonly IProductService _productService;
        private readonly IProductImportService _importService;

        public AddProductViewModel(IProductService productService, IProductImportService importService)
        {
            _productService = productService;
            _importService = importService;
            SelectedType = ProductType.Medicine;
            SaveCommand = new AsyncRelayCommand(async _ => await SaveAsync());
            ClearCommand = new RelayCommand(Clear);
            ImportMedicineCommand = new AsyncRelayCommand(async _ => await ImportAsync(ProductType.Medicine));
            ImportGroceryCommand = new AsyncRelayCommand(async _ => await ImportAsync(ProductType.Grocery));
        }

        private ProductType _selectedType;
        public ProductType SelectedType
        {
            get => _selectedType;
            set
            {
                SetProperty(ref _selectedType, value);
                OnPropertyChanged(nameof(IsMedicine));
                OnPropertyChanged(nameof(IsGrocery));
            }
        }

        public bool IsMedicine => SelectedType == ProductType.Medicine;
        public bool IsGrocery => SelectedType == ProductType.Grocery;

        // Medicine fields
        public string Category { get; set; } = string.Empty;
        public string GenericName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string? Strength { get; set; }
        public string? DosageForm { get; set; }
        public string? BatchNo { get; set; }
        public DateTime? MedicineExpDate { get; set; }
        public DateTime? MedicineMfdDate { get; set; }
        public string? Packing { get; set; }
        public int NoOfUnits { get; set; }
        public decimal UnitPrice { get; set; }
        public int UnitsPerPack { get; set; }
        public int NoOfPacks { get; set; }
        public decimal PackPrice { get; set; }
        public decimal TotalValue { get; set; }

        // Grocery fields
        public string ItemType { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string? Speciality { get; set; }
        public string? Size { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
        public decimal Total { get; set; }
        public DateTime? GroceryExdDate { get; set; }
        public DateTime? GroceryMfdDate { get; set; }
        public string? OutColour { get; set; }
        public string? Note { get; set; }

        // Shared operational fields retained for compatibility
        public int QuantityOnHand { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public DateTime? MfdDate { get; set; }
        public DateTime? ExpDate { get; set; }

        public AsyncRelayCommand SaveCommand { get; }
        public RelayCommand ClearCommand { get; }
        public AsyncRelayCommand ImportMedicineCommand { get; }
        public AsyncRelayCommand ImportGroceryCommand { get; }

        private void Clear(object? _ = null)
        {
            Category = string.Empty;
            GenericName = string.Empty;
            BrandName = string.Empty;
            Strength = null;
            DosageForm = null;
            BatchNo = null;
            MedicineExpDate = null;
            MedicineMfdDate = null;
            Packing = null;
            NoOfUnits = 0;
            UnitPrice = 0m;
            UnitsPerPack = 0;
            NoOfPacks = 0;
            PackPrice = 0m;
            TotalValue = 0m;

            ItemType = string.Empty;
            Brand = string.Empty;
            Speciality = null;
            Size = null;
            Price = 0m;
            Count = 0;
            Total = 0m;
            GroceryExdDate = null;
            GroceryMfdDate = null;
            OutColour = null;
            Note = null;

            BuyingPrice = 0m;
            SellingPrice = 0m;
            MfdDate = null;
            ExpDate = null;
            QuantityOnHand = 0;

            OnPropertyChanged(string.Empty);
        }

        private bool Validate(out string error)
        {
            error = string.Empty;

            if (SelectedType == ProductType.Medicine)
            {
                if (string.IsNullOrWhiteSpace(Category)) { error = "Category is required."; return false; }
                if (string.IsNullOrWhiteSpace(GenericName)) { error = "Generic name is required."; return false; }
                if (string.IsNullOrWhiteSpace(BrandName)) { error = "Brand name is required."; return false; }
                if (string.IsNullOrWhiteSpace(BatchNo)) { error = "Batch No is required."; return false; }
                if (NoOfUnits < 0 || UnitsPerPack < 0 || NoOfPacks < 0) { error = "Medicine quantities must be non-negative."; return false; }
                if (UnitPrice < 0 || PackPrice < 0 || TotalValue < 0) { error = "Medicine prices must be non-negative."; return false; }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(ItemType)) { error = "Item type is required."; return false; }
                if (string.IsNullOrWhiteSpace(Brand)) { error = "Brand is required."; return false; }
                if (string.IsNullOrWhiteSpace(Size)) { error = "Size is required."; return false; }
                if (Count < 0) { error = "Count must be >= 0."; return false; }
                if (Price < 0 || Total < 0) { error = "Grocery prices must be non-negative."; return false; }
            }

            return true;
        }

        private Product CreateProduct()
        {
            var product = new Product
            {
                Type = SelectedType,
                QuantityOnHand = QuantityOnHand,
                BuyingPrice = BuyingPrice,
                SellingPrice = SellingPrice,
                MfdDate = MfdDate.HasValue ? DateOnly.FromDateTime(MfdDate.Value) : null,
                ExpDate = ExpDate.HasValue ? DateOnly.FromDateTime(ExpDate.Value) : null,
            };

            if (SelectedType == ProductType.Medicine)
            {
                product.Category = Category;
                product.GenericName = GenericName;
                product.BrandName = BrandName;
                product.DosageForm = DosageForm;
                product.Strength = Strength;
                product.BatchNo = BatchNo;
                product.MfdDate = MedicineMfdDate.HasValue ? DateOnly.FromDateTime(MedicineMfdDate.Value) : product.MfdDate;
                product.ExpDate = MedicineExpDate.HasValue ? DateOnly.FromDateTime(MedicineExpDate.Value) : null;
                product.Packing = Packing;
                product.NoOfUnits = NoOfUnits;
                product.UnitPrice = UnitPrice;
                product.UnitsPerPack = UnitsPerPack;
                product.NoOfPacks = NoOfPacks;
                product.PackPrice = PackPrice;
                product.TotalValue = TotalValue;
                product.QuantityOnHand = NoOfUnits > 0 && NoOfPacks > 0 ? NoOfUnits * NoOfPacks : NoOfUnits;
                product.SellingPrice = UnitPrice;
                product.BuyingPrice = PackPrice;
            }
            else
            {
                product.ItemType = ItemType;
                product.Brand = Brand;
                product.Speciality = Speciality;
                product.Size = Size;
                product.Price = Price;
                product.Count = Count;
                product.Total = Total;
                product.ExdDate = GroceryExdDate.HasValue ? DateOnly.FromDateTime(GroceryExdDate.Value) : null;
                product.MfdDate = GroceryMfdDate.HasValue ? DateOnly.FromDateTime(GroceryMfdDate.Value) : product.MfdDate;
                product.OutColour = OutColour;
                product.Note = Note;
                product.QuantityOnHand = Count;
                product.SellingPrice = Price;
                product.BuyingPrice = Total;
            }

            return product;
        }

        private async Task SaveAsync()
        {
            if (!Validate(out var error))
            {
                await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show(error, "Validation", MessageBoxButton.OK, MessageBoxImage.Warning));
                return;
            }

            var product = CreateProduct();

            if (SelectedType == ProductType.Medicine)
            {
                await _productService.AddMedicineAsync(product);
            }
            else
            {
                await _productService.AddGroceryAsync(product);
            }

            await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show("Product saved", "Success", MessageBoxButton.OK, MessageBoxImage.Information));
            Clear();
        }

        private async Task ImportAsync(ProductType type)
        {
            var dialog = new OpenFileDialog
            {
                Title = type == ProductType.Medicine ? "Import medicine Excel" : "Import grocery Excel",
                Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                var imported = type == ProductType.Medicine
                    ? await _importService.ImportMedicinesAsync(dialog.FileName)
                    : await _importService.ImportGroceriesAsync(dialog.FileName);

                await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show($"Imported {imported} item(s).", "Import complete", MessageBoxButton.OK, MessageBoxImage.Information));
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show(ex.Message, "Import failed", MessageBoxButton.OK, MessageBoxImage.Error));
            }
        }
    }
}
