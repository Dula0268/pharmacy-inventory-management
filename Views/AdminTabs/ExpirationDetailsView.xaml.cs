using System.Windows.Controls;

namespace PharmacyInventory.Views.AdminTabs
{
    public partial class ExpirationDetailsView : UserControl
    {
        public ExpirationDetailsView()
        {
            InitializeComponent();
            DataContext = App.Services?.GetService(typeof(ViewModels.AdminTabs.ExpirationDetailsViewModel));
        }
    }
}
