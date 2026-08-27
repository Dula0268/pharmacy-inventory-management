using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using PharmacyInventory.Commands;
using PharmacyInventory.Data;
using PharmacyInventory.Helpers;
using PharmacyInventory.Models;

namespace PharmacyInventory.ViewModels.AdminTabs
{
    public class SettingsViewModel : ViewModels.BaseViewModel
    {
        private readonly PharmacyDbContext _db;

        public SettingsViewModel(PharmacyDbContext db)
        {
            _db = db;
            Users = new ObservableCollection<UserRow>();
            AddUserCommand = new AsyncRelayCommand(async _ => await AddUserAsync());
            RefreshCommand = new AsyncRelayCommand(async _ => await LoadAsync());
            DeleteUserCommand = new AsyncRelayCommand(async p => await DeleteUserAsync(p));
            SelectedRole = UserRole.Cashier;
        }

        // Add user form
        public string NewUsername { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public UserRole SelectedRole { get; set; }

        public ObservableCollection<UserRow> Users { get; }

        public AsyncRelayCommand AddUserCommand { get; }
        public AsyncRelayCommand RefreshCommand { get; }
        public AsyncRelayCommand DeleteUserCommand { get; }

        public class UserRow
        {
            private readonly SettingsViewModel _vm;
            public UserRow(SettingsViewModel vm, AppUser u)
            {
                _vm = vm;
                Id = u.Id;
                Username = u.Username;
                Role = u.Role;
                _isActive = u.IsActive;
            }

            public int Id { get; }
            public string Username { get; }
            public UserRole Role { get; }

            private bool _isActive;
            public bool IsActive
            {
                get => _isActive;
                set
                {
                    if (_isActive == value) return;
                    _isActive = value;
                    _ = _vm.SetUserActiveAsync(this, value);
                }
            }
        }

        public async Task LoadAsync()
        {
            try
            {
                var list = await _db.Users.AsNoTracking().OrderBy(u => u.Username).ToListAsync();
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Users.Clear();
                    foreach (var u in list) Users.Add(new UserRow(this, u));
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show(ex.Message, "Users Load Error", MessageBoxButton.OK, MessageBoxImage.Error));
            }
        }

        private async Task DeleteUserAsync(object? param)
        {
            if (param is not UserRow row) return;

            try
            {
                var confirm = false;
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    var res = MessageBox.Show($"Delete user '{row.Username}'?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    confirm = res == MessageBoxResult.Yes;
                });

                if (!confirm) return;

                var user = await _db.Users.FindAsync(row.Id);
                if (user == null) return;

                // Prevent deleting last active admin
                if (user.Role == UserRole.Admin)
                {
                    var activeAdmins = await _db.Users.CountAsync(u => u.Role == UserRole.Admin && u.IsActive);
                    if (activeAdmins <= 1)
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show("Cannot delete the last active admin.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning));
                        return;
                    }
                }

                _db.Users.Remove(user);
                await _db.SaveChangesAsync();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    var existing = Users.FirstOrDefault(u => u.Id == row.Id);
                    if (existing != null) Users.Remove(existing);
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show(ex.Message, "Delete User Error", MessageBoxButton.OK, MessageBoxImage.Error));
                await LoadAsync();
            }
        }

        private async Task AddUserAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewUsername))
                {
                    await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show("Username is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning));
                    return;
                }
                if (string.IsNullOrEmpty(NewPassword) || NewPassword.Length < 4)
                {
                    await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show("Password is required (min 4 chars).", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning));
                    return;
                }

                var exists = await _db.Users.AnyAsync(u => u.Username == NewUsername);
                if (exists)
                {
                    await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show("Username already exists.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning));
                    return;
                }

                var hash = PasswordHasher.Hash(NewPassword);
                var user = new AppUser { Username = NewUsername, PasswordHash = hash, Role = SelectedRole, IsActive = true };
                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Users.Add(new UserRow(this, user));
                    NewUsername = string.Empty;
                    NewPassword = string.Empty;
                    OnPropertyChanged(nameof(NewUsername));
                    OnPropertyChanged(nameof(NewPassword));
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show(ex.Message, "Add User Error", MessageBoxButton.OK, MessageBoxImage.Error));
            }
        }

        public async Task SetUserActiveAsync(UserRow row, bool isActive)
        {
            try
            {
                var user = await _db.Users.FindAsync(row.Id);
                if (user == null) return;

                // If disabling an admin, ensure at least one admin remains active
                if (!isActive && user.Role == UserRole.Admin)
                {
                    var activeAdmins = await _db.Users.CountAsync(u => u.Role == UserRole.Admin && u.IsActive);
                    if (activeAdmins <= 1)
                    {
                        // revert
                        await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show("Cannot disable the last active admin.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning));
                        // reload to reset UI
                        await LoadAsync();
                        return;
                    }
                }

                user.IsActive = isActive;
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() => MessageBox.Show(ex.Message, "Update User Error", MessageBoxButton.OK, MessageBoxImage.Error));
                await LoadAsync();
            }
        }
    }
}
 
