using System.Windows;
using PharmacyInventory.ViewModels;
using PharmacyInventory.Views;

namespace PharmacyInventory.Views
{
    public partial class CashierWindow : Window
    {
        public CashierWindow()
        {
            InitializeComponent();
            DataContext = App.Services?.GetService(typeof(CashierViewModel));
        }

        private void OnLogoutClick(object sender, RoutedEventArgs e)
        {
            var login = App.Services?.GetService(typeof(LoginWindow)) as LoginWindow;
            if (login != null)
            {
                login.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                login.Show();
            }

            this.Close();
        }
    }
}
