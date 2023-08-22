using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Collections;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Text;

namespace IntermediateAPI.Utilities
{
    public static class IdentityPasswordHasher
    {
        public static string GenerateIdentityHash(string password, string salt)
        { 
            const KeyDerivationPrf Pbkdf2Prf = KeyDerivationPrf.HMACSHA1; // default for Rfc2898DeriveBytes
            const int Pbkdf2IterCount = 1000; // default for Rfc2898DeriveBytes
            const int Pbkdf2SubkeyLength = 256 / 8; // 256 bits
            const int SaltSize = 128 / 8; // 128 bits

            byte[] saltStringBytes = Convert.FromBase64String(salt);

            var saltArray = new byte[SaltSize];

            if (saltStringBytes.Length != SaltSize)
            {
                throw new InvalidEnumArgumentException("Invalid salt : Array doesn't have required size");
            }

            Buffer.BlockCopy(saltStringBytes, 0, saltArray, 0, saltStringBytes.Length);

            byte[] subkey = KeyDerivation.Pbkdf2(password, saltArray, Pbkdf2Prf, Pbkdf2IterCount, Pbkdf2SubkeyLength);

            var outputBytes = new byte[1 + SaltSize + Pbkdf2SubkeyLength];
            outputBytes[0] = 0x00; // format marker
            Buffer.BlockCopy(saltArray, 0, outputBytes, 1, SaltSize);
            Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SaltSize, Pbkdf2SubkeyLength);

            return Convert.ToBase64String(outputBytes);
        }
    }
}
