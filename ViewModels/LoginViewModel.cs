using System.Threading.Tasks;
using System.Windows;
using PharmacyInventory.Models;
using PharmacyInventory.Services;

namespace PharmacyInventory.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _auth;

        private string _username = "";
        private string _password = "";
        private string _errorMessage = "";

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(ErrorVisibility)); }
        }

        public Visibility ErrorVisibility => string.IsNullOrWhiteSpace(ErrorMessage) ? Visibility.Collapsed : Visibility.Visible;

        public LoginViewModel(IAuthService auth)
        {
            _auth = auth;
        }

        public async Task<AppUser?> TryLoginAsync()
        {
            ErrorMessage = "";

            var user = await _auth.LoginAsync(Username, Password);
            if (user == null)
            {
                ErrorMessage = "Invalid username or password.";
                return null;
            }

            return user;
        }
    }
}
