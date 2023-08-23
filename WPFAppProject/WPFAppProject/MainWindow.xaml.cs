using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.Collections;
using System.Drawing;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Security.Policy;

namespace WPFAppProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string testPassword = "CY9rzUYh03PK3k6DJie09g==";

        public MainWindow()
        {
            InitializeComponent();
        }

        //Set window minimum size
        private void Set_Minimums(object sender, EventArgs e)
        {
            MinWidth = ActualWidth;
            MinHeight = ActualHeight;
        }

        //Salt a password
        static byte[] GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[32]; // 32 bytes for a strong salt
                rng.GetBytes(salt);
                return salt;
            }
        }

        //Encode a password
        static string encode(string password, byte[] salt)
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

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(usernameBox.Text))
            {
                MessageBox.Show("Warning: Username Required");
                return;
            }

            if (string.IsNullOrEmpty(passwordBox.Password))
            {
                MessageBox.Show("Warning: Password Required");
                return;
            }

            byte[] salt = GenerateSalt();
            string hashedPassword = encode(passwordBox.Password, salt);

            MessageBox.Show("Salt is: " + Convert.ToBase64String(salt) + ", encrypted password is: " + hashedPassword);
            using StreamWriter file = new($"TestPassword.txt");
            file.WriteLine(usernameBox.Text);
            file.WriteLine(Convert.ToBase64String(salt));
            file.WriteLine(hashedPassword);
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            StreamReader sr = new StreamReader($"TestPassword.txt");
            var line = sr.ReadLine();
            line = sr.ReadLine();
            var salt = Convert.FromBase64String(line);
            line = sr.ReadLine();
            var hashedPassword = line;

            string encoded = encode(passwordBox.Password, salt);

            if (encoded.Equals(hashedPassword))
            {
                MessageBox.Show("Correct Password");
            }

            sr.Close();
        }
    }
}
