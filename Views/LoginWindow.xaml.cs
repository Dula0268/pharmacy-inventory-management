using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PharmacyInventory.Models;
using PharmacyInventory.ViewModels;
using PharmacyInventory.Views;

namespace PharmacyInventory.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel? _vm;

        // Parameterless constructor for designer and XAML tools
        public LoginWindow()
        {
            InitializeComponent();
        }

        // Constructor used by DI to supply the ViewModel
        public LoginWindow(LoginViewModel vm) : this()
        {
            _vm = vm ?? throw new ArgumentNullException(nameof(vm));
            DataContext = _vm;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_vm != null && PasswordBox != null)
                _vm.Password = PasswordBox.Password;
        }

        private void UsernameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PasswordBox?.Focus();
                e.Handled = true;
            }
        }

        private async void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                await PerformLoginAsync();
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            await PerformLoginAsync();
        }

        private async Task PerformLoginAsync()
        {
            try
            {
                LoginButton.IsEnabled = false;
                if (_vm is null)
                {
                    MessageBox.Show("Login view model is not available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var user = await _vm.TryLoginAsync();
                if (!string.IsNullOrEmpty(_vm.ErrorMessage) || user is null)
                {
                    ErrorText.Visibility = Visibility.Visible;
                }
                else
                {
                    ErrorText.Visibility = Visibility.Collapsed;
                    
                    // Show the appropriate main window based on user role
                    Window? mainWindow = null;
                    if (user.Role == PharmacyInventory.Models.UserRole.Admin)
                    {
                        mainWindow = App.Services?.GetService(typeof(AdminWindow)) as AdminWindow;
                    }
                    else if (user.Role == PharmacyInventory.Models.UserRole.Cashier)
                    {
                        mainWindow = App.Services?.GetService(typeof(CashierWindow)) as CashierWindow;
                    }

                    if (mainWindow != null)
                    {
                        // Set to maximized before showing
                        mainWindow.WindowState = WindowState.Maximized;
                        mainWindow.Show();
                        
                        // Set the new window as MainWindow so app doesn't close when this window closes
                        Application.Current.MainWindow = mainWindow;
                        
                        // Now close the login window
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Could not load main window.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Login error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoginButton.IsEnabled = true;
            }
        }
    }
}