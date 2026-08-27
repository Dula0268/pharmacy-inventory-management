using System.Windows.Controls;

namespace PharmacyInventory.Views.AdminTabs
{
    public partial class ReportsView : UserControl
    {
        public ReportsView()
        {
            InitializeComponent();
            DataContext = App.Services?.GetService(typeof(ViewModels.AdminTabs.ReportsViewModel));
        }
    }
}
