using System.Windows;
using PharmacyInventory.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using PharmacyInventory.Views;

namespace PharmacyInventory.Views
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            DataContext = App.Services?.GetService(typeof(AdminWindowViewModel));
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
