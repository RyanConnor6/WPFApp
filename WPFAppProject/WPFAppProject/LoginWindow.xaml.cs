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
    public partial class LoginWindow : Window
    {
        PasswordHandler passwordHandler = PasswordHandler.getInstance();

        public LoginWindow()
        {
            InitializeComponent();
        }

        //Set window minimum size
        private void Set_Minimums(object sender, EventArgs e)
        {
            MinWidth = ActualWidth;
            MinHeight = ActualHeight;
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

            byte[] salt = passwordHandler.GenerateSalt();
            string hashedPassword = passwordHandler.encode(passwordBox.Password, salt);

            using StreamWriter file = new($"TestPassword.txt");
            file.WriteLine(usernameBox.Text);
            file.WriteLine(Convert.ToBase64String(salt));
            file.WriteLine(hashedPassword);
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(usernameBox.Text))
            {
                MessageBox.Show("Warning: Enter Username");
                return;
            }

            if (string.IsNullOrEmpty(passwordBox.Password))
            {
                MessageBox.Show("Warning: Enter Password");
                return;
            }

            StreamReader sr = new StreamReader($"TestPassword.txt");
            var line = sr.ReadLine();
            line = sr.ReadLine();
            var salt = Convert.FromBase64String(line);
            line = sr.ReadLine();
            var hashedPassword = line;

            string encoded = passwordHandler.encode(passwordBox.Password, salt);

            if (encoded.Equals(hashedPassword))
            {
                MessageBox.Show("Correct Password");
                return;
            }

            MessageBox.Show("Incorrect Password");

            sr.Close();
        }
    }
}
