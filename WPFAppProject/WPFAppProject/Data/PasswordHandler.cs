using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WPFAppProject.Data
{
    //Password handler singleton
    public sealed class PasswordHandler
    {
        //Singelton code
        private static PasswordHandler reference;

        public PasswordHandler() { }

        //Create the only instance of handler
        public static PasswordHandler getInstance()
        {
            if (reference == null)
                reference = new PasswordHandler();

            return reference;
        }

        //Salt a password
        public byte[] GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[32]; // 32 bytes for a strong salt
                rng.GetBytes(salt);
                return salt;
            }
        }

        //Encode a password
        public string encode(string password, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

                // Concatenate password and salt
                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

                byte[] hashBytes = sha256.ComputeHash(saltedPassword);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
