using System;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PharmacyInventory.Models;
using PharmacyInventory.Helpers;

namespace PharmacyInventory.Data
{
    public class PharmacyDbContext : DbContext
    {
        public PharmacyDbContext(DbContextOptions<PharmacyDbContext> options) : base(options)
        {
        }

        public DbSet<InventoryItem> InventoryItems { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<AppUser> Users { get; set; } = null!;
        public DbSet<Sale> Sales { get; set; } = null!;
        public DbSet<SaleItem> SaleItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ValueConverter for nullable DateOnly <-> string (yyyy-MM-dd) for SQLite
            var dateOnlyConverter = new ValueConverter<DateOnly?, string?>(
                d => d.HasValue ? d.Value.ToString("yyyy-MM-dd") : null,
                s => string.IsNullOrEmpty(s) ? null : DateOnly.ParseExact(s, "yyyy-MM-dd"));

            modelBuilder.Entity<Product>(b =>
            {
                b.Property(p => p.MfdDate)
                    .HasConversion(dateOnlyConverter)
                    .HasColumnType("TEXT");

                b.Property(p => p.ExpDate)
                    .HasConversion(dateOnlyConverter)
                    .HasColumnType("TEXT");

                b.Property(p => p.ExdDate)
                    .HasConversion(dateOnlyConverter)
                    .HasColumnType("TEXT");

                // Indexes to speed lookups
                b.HasIndex(p => p.Category);
                b.HasIndex(p => p.BrandName);
                b.HasIndex(p => p.GenericName);
                b.HasIndex(p => p.ItemType);
                b.HasIndex(p => p.Brand);
            });

            modelBuilder.Entity<AppUser>(b =>
            {
                b.HasIndex(u => u.Username).IsUnique();
            });

            // Seed initial users.
            // PasswordHash values are pinned literals (not a live PasswordHasher.Hash() call):
            // Hash() salts randomly, so calling it here would make EF regenerate a "changed"
            // seed hash on every migration scaffold, producing spurious UpdateData migrations
            // that can drift out of sync with the actual default passwords (admin@123 / pass@123).
            var admin = new AppUser
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "ig0za9XUfGfivqDJZVwMNw==.uQFoojQzPCOSISW3e1fHJXTyNGqnv9u6mPd3/lvE4f4=",
                Role = UserRole.Admin,
                IsActive = true
            };

            var cashier = new AppUser
            {
                Id = 2,
                Username = "cashier",
                PasswordHash = "SOKIlfjxDfqkENFT+Xu1tQ==.x/P2E9uTI+l04R9RioWn3SCwcSmLg4csIgSzTmiS3Mw=",
                Role = UserRole.Cashier,
                IsActive = true
            };

            modelBuilder.Entity<AppUser>().HasData(admin, cashier);

            base.OnModelCreating(modelBuilder);
        }
    }
}
