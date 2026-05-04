using System;
using System.Linq;
using System.Security.Cryptography;

namespace SEGREWARDS_PROJECT.Infrastructure
{
    /// <summary>
    /// PBKDF2 password hashing (no extra NuGet packages).
    /// </summary>
    public static class PasswordHasher
    {
        private const int SaltSizeBytes = 16;
        private const int KeySizeBytes = 32;
        private const int Iterations = 100_000;

        public static byte[] CreateSalt()
        {
            var salt = new byte[SaltSizeBytes];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }

        public static byte[] Hash(string password, byte[] salt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (salt == null || salt.Length < 8) throw new ArgumentException("Salt is required.", nameof(salt));

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                return pbkdf2.GetBytes(KeySizeBytes);
            }
        }

        public static bool Verify(string password, byte[] salt, byte[] expectedHash)
        {
            if (expectedHash == null || expectedHash.Length == 0) return false;
            var actual = Hash(password, salt);
            return FixedTimeEquals(actual, expectedHash);
        }

        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            return a.SequenceEqual(b);
        }
    }
}
