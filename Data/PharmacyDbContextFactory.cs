using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PharmacyInventory.Data
{
    public class PharmacyDbContextFactory : IDesignTimeDbContextFactory<PharmacyDbContext>
    {
        public PharmacyDbContext CreateDbContext(string[] args)
        {
            // Use the same rule as runtime, but design-time needs a stable path.
            // Put db in the project folder for migrations:
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "pharmacy.db");

            var options = new DbContextOptionsBuilder<PharmacyDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;

            return new PharmacyDbContext(options);
        }
    }
}
