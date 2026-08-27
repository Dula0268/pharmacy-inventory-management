using System;
using System.Security.Cryptography;

namespace PharmacyInventory.Helpers
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16; // 16 bytes
        private const int KeySize = 32;  // 32 bytes
        private const int Iterations = 100_000;

        public static string Hash(string password)
        {
            if (password is null) throw new ArgumentNullException(nameof(password));

            var salt = new byte[SaltSize];
            RandomNumberGenerator.Fill(salt);

            using var derive = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var key = derive.GetBytes(KeySize);

            var saltB64 = Convert.ToBase64String(salt);
            var keyB64 = Convert.ToBase64String(key);

            return string.Concat(saltB64, ".", keyB64);
        }

        public static bool Verify(string password, string stored)
        {
            if (password is null) throw new ArgumentNullException(nameof(password));
            if (stored is null) throw new ArgumentNullException(nameof(stored));

            var parts = stored.Split('.', 2);
            if (parts.Length != 2)
                return false;

            byte[] salt, expectedKey;
            try
            {
                salt = Convert.FromBase64String(parts[0]);
                expectedKey = Convert.FromBase64String(parts[1]);
            }
            catch
            {
                return false;
            }

            using var derive = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var actualKey = derive.GetBytes(expectedKey.Length);

            return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
        }
    }
}
