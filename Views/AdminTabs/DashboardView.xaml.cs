using System.Windows.Controls;

namespace PharmacyInventory.Views.AdminTabs
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            DataContext = App.Services?.GetService(typeof(ViewModels.AdminTabs.DashboardViewModel));
        }
    }
}
