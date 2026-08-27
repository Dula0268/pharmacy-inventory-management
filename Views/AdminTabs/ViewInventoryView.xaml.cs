using System.Windows.Controls;

namespace PharmacyInventory.Views.AdminTabs
{
    public partial class ViewInventoryView : UserControl
    {
        public ViewInventoryView()
        {
            InitializeComponent();
            DataContext = App.Services?.GetService(typeof(ViewModels.AdminTabs.ViewInventoryViewModel));
        }
    }
}
