using System.Windows.Controls;

namespace PharmacyInventory.Views.AdminTabs
{
    public partial class AddProductView : UserControl
    {
        public AddProductView()
        {
            InitializeComponent();
            DataContext = App.Services?.GetService(typeof(ViewModels.AdminTabs.AddProductViewModel));
        }
    }
}
