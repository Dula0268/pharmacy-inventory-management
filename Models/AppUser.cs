using System.Collections.Generic;

namespace PharmacyInventory.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
