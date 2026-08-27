using System;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PharmacyInventory.Data;
using PharmacyInventory.Services;
using PharmacyInventory.ViewModels;
using PharmacyInventory.Views;

namespace PharmacyInventory
{
    public partial class App : Application
    {
        private IHost? _host;

        // Optional: allows access in places you don't inject (prefer DI though)
        public static IServiceProvider? Services { get; private set; }

        public App()
        {
            InitializeComponent();

            // Never let startup failures be silent
            DispatcherUnhandledException += (_, ex) =>
            {
                MessageBox.Show(ex.Exception.ToString(), "Unhandled UI Exception");
                ex.Handled = true;
            };
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    var dbPath = Path.Combine(AppContext.BaseDirectory, "pharmacy.db");
                    services.AddDbContext<PharmacyDbContext>(options =>
                        options.UseSqlite($"Data Source={dbPath}"));

                    // ✅ IMPORTANT: DbContext-based services must NOT be Singleton
                    services.AddScoped<IAuthService, AuthService>();
                    services.AddScoped<IProductService, ProductService>();
                    services.AddScoped<IProductImportService, ProductImportService>();
                    services.AddScoped<ISalesService, SalesService>();
                    services.AddScoped<IReportService, ReportService>();

                    // ViewModels
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<CashierViewModel>();
                    services.AddTransient<AdminWindowViewModel>();

                    // Windows
                    services.AddTransient<LoginWindow>();
                    services.AddTransient<AdminWindow>();
                    services.AddTransient<CashierWindow>();

                    // (Register admin tab VMs if you already use them)
                    services.AddTransient<ViewModels.AdminTabs.DashboardViewModel>();
                    services.AddTransient<ViewModels.AdminTabs.AddProductViewModel>();
                    services.AddTransient<ViewModels.AdminTabs.SearchProductsViewModel>();
                    services.AddTransient<ViewModels.AdminTabs.ExpirationDetailsViewModel>();
                    services.AddTransient<ViewModels.AdminTabs.ExpirationViewModel>();
                    services.AddTransient<ViewModels.AdminTabs.ViewInventoryViewModel>();
                    services.AddTransient<ViewModels.AdminTabs.ReportsViewModel>();
                    services.AddTransient<ViewModels.AdminTabs.SettingsViewModel>();
                })
                .Build();

            Services = _host.Services;

            // Apply migrations BEFORE showing UI (and show errors if it fails)
            try
            {
                using var scope = _host.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<PharmacyDbContext>();
                await db.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Database migration failed");
                Shutdown();
                return;
            }

            await _host.StartAsync();

            // Show Login Window (no swallowing exceptions)
            try
            {
                var login = _host.Services.GetRequiredService<LoginWindow>();
                login.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                MainWindow = login;
                login.Show();
                login.Activate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Startup error");
                Shutdown();
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }
            base.OnExit(e);
        }
    }
}
