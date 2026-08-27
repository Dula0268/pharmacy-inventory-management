using System.Windows.Controls;

namespace PharmacyInventory.Views.AdminTabs
{
    public partial class SearchProductsView : UserControl
    {
        public SearchProductsView()
        {
            InitializeComponent();
            DataContext = App.Services?.GetService(typeof(ViewModels.AdminTabs.SearchProductsViewModel));
        }
    }
}
