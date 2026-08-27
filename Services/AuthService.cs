using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PharmacyInventory.Data;
using PharmacyInventory.Helpers;
using PharmacyInventory.Models;

namespace PharmacyInventory.Services
{
    public class AuthService : IAuthService
    {
        private readonly PharmacyDbContext _db;

        public AuthService(PharmacyDbContext db)
        {
            _db = db;
        }

        public async Task<AppUser?> LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            // simple exact match (you can later make it case-insensitive)
            var user = await _db.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user is null || !user.IsActive)
                return null;

            return PasswordHasher.Verify(password, user.PasswordHash) ? user : null;
        }
    }
}
