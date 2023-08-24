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

        //Constructor, empty as there should only be one
        public PasswordHandler() { }

        //Create the only instance of handler
        public static PasswordHandler getInstance()
        {
            if (reference == null)
                reference = new PasswordHandler();

            return reference;
        }

        //Create some salt
        public byte[] GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[32]; //Strong 32 byte salt
                rng.GetBytes(salt);
                return salt;
            }
        }

        //Encode a password
        public string encode(string password, byte[] salt)
        {
            //Hash using sha256
            using (var sha256 = SHA256.Create())
            {
                //UTF8 format
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

                //Concatenate password and salt
                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

                //Get hash of the salted password
                byte[] hashBytes = sha256.ComputeHash(saltedPassword);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
