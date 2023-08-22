using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Collections;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Text;

namespace IntermediateAPI.Utilities
{
    public static class IdentityPasswordHasher
    {

        public static (string Salt, string PasswordHash) GenerateIdentityHash(string password)
        {
            const KeyDerivationPrf Pbkdf2Prf = KeyDerivationPrf.HMACSHA1; // default for Rfc2898DeriveBytes
            const int Pbkdf2IterCount = 1000; // default for Rfc2898DeriveBytes
            const int Pbkdf2SubkeyLength = 256 / 8; // 256 bits
            const int SaltSize = 128 / 8; // 128 bits

            var salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            byte[] subkey = KeyDerivation.Pbkdf2(password, salt, Pbkdf2Prf, Pbkdf2IterCount, Pbkdf2SubkeyLength);

            var outputBytes = new byte[1 + SaltSize + Pbkdf2SubkeyLength];
            outputBytes[0] = 0x00; // format marker
            Buffer.BlockCopy(salt, 0, outputBytes, 1, SaltSize);
            Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SaltSize, Pbkdf2SubkeyLength);
            return (Convert.ToBase64String(salt),Convert.ToBase64String(outputBytes));
        }

        public static (string Salt, string PasswordHash) GenerateIdentityHash(string password, string salt)
        { 
            const KeyDerivationPrf Pbkdf2Prf = KeyDerivationPrf.HMACSHA1; // default for Rfc2898DeriveBytes
            const int Pbkdf2IterCount = 1000; // default for Rfc2898DeriveBytes
            const int Pbkdf2SubkeyLength = 256 / 8; // 256 bits
            const int SaltSize = 128 / 8; // 128 bits

            var saltArray = new byte[SaltSize];
            //using (var rng = RandomNumberGenerator.Create())
            //{
            //    rng.GetBytes(saltArray);
            //}
            byte[] saltStringBytes = Convert.FromBase64String(salt);

            Buffer.BlockCopy(saltStringBytes, 0, saltArray, 0, saltStringBytes.Length);

            


            byte[] subkey = KeyDerivation.Pbkdf2(password, saltArray, Pbkdf2Prf, Pbkdf2IterCount, Pbkdf2SubkeyLength);

            var outputBytes = new byte[1 + SaltSize + Pbkdf2SubkeyLength];
            outputBytes[0] = 0x00; // format marker
            Buffer.BlockCopy(saltArray, 0, outputBytes, 1, SaltSize);
            Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SaltSize, Pbkdf2SubkeyLength);
            return (Convert.ToBase64String(saltArray), Convert.ToBase64String(outputBytes));
        }


        public static string ExtractSalt(string hashedPassword)
        {
            const int SaltSize = 128 / 8; // 128 bits

            byte[] decodedHashedPassword = Convert.FromBase64String(hashedPassword);

            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(decodedHashedPassword, 1, salt, 0, salt.Length);

            return Convert.ToBase64String(salt);
        }

        public static (string Salt, string PasswordHash) VerifyPassword(string hashedPassword, string password)
        {
            byte[] decodedHashedPassword = Convert.FromBase64String(hashedPassword);

            const KeyDerivationPrf Pbkdf2Prf = KeyDerivationPrf.HMACSHA1; // default for Rfc2898DeriveBytes
            const int Pbkdf2IterCount = 1000; // default for Rfc2898DeriveBytes
            const int Pbkdf2SubkeyLength = 256 / 8; // 256 bits
            const int SaltSize = 128 / 8; // 128 bits


            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(decodedHashedPassword, 1, salt, 0, salt.Length);

            byte[] expectedSubkey = new byte[Pbkdf2SubkeyLength];
            Buffer.BlockCopy(decodedHashedPassword, 1 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

            // Hash the incoming password and verify it
            byte[] actualSubkey = KeyDerivation.Pbkdf2(password, salt, Pbkdf2Prf, Pbkdf2IterCount, Pbkdf2SubkeyLength);

            var outputBytes = new byte[1 + SaltSize + Pbkdf2SubkeyLength];
            outputBytes[0] = 0x00; // format marker
            Buffer.BlockCopy(salt, 0, outputBytes, 1, SaltSize);
            Buffer.BlockCopy(actualSubkey, 0, outputBytes, 1 + SaltSize, Pbkdf2SubkeyLength);
            return (Convert.ToBase64String(salt), Convert.ToBase64String(outputBytes));
        }
    }
}
