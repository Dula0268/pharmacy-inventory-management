using System.Windows.Controls;

namespace PharmacyInventory.Views.AdminTabs
{
    public partial class InventoryView : UserControl
    {
        public InventoryView()
        {
            InitializeComponent();
            DataContext = App.Services?.GetService(typeof(ViewModels.AdminTabs.InventoryViewModel));
        }
    }
}
