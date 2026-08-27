using System.Windows;
using System.Windows.Controls;

namespace PharmacyInventory.Views.AdminTabs
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            DataContext = App.Services?.GetService(typeof(ViewModels.AdminTabs.SettingsViewModel));
        }

        private async void OnAddClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.AdminTabs.SettingsViewModel vm)
            {
                // transfer password from PasswordBox
                vm.NewPassword = pwdBox.Password;
                await vm.AddUserCommand.ExecuteAsync(null);
                pwdBox.Password = string.Empty;
            }
        }
    }
}
