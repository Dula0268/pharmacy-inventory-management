using System.Windows.Controls;

namespace PharmacyInventory.Views.AdminTabs
{
    public partial class ExpirationView : UserControl
    {
        public ExpirationView()
        {
            InitializeComponent();
            DataContext = App.Services?.GetService(typeof(ViewModels.AdminTabs.ExpirationViewModel));
        }
    }
}
