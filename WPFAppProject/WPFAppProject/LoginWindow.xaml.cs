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
using WPFAppProject.Data;
using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations.Schema;
using static Google.Rpc.Help.Types;

namespace WPFAppProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        PasswordHandler passwordHandler = PasswordHandler.getInstance();
        UserHandler userHandler = UserHandler.getInstance();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginWindow_Load(object sender, EventArgs e)
        {
            //Onload activities here
            userHandler.accessDatabase();
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

            if (usernameBox.Text.Length < 5)
            {
                MessageBox.Show("Warning: Username Must Be 5 Or More Characters In Length");
                return;
            }

            if (usernameBox.Text.Length > 20)
            {
                MessageBox.Show("Warning: Username Must Be 20 Or Less Characters In Length");
                return;
            }

            if (usernameBox.Text.Contains(" "))
            {
                MessageBox.Show("Warning: Username Cannot Contain Spaces");
                return;
            }

            byte[] salt = passwordHandler.GenerateSalt();
            string hashedPassword = passwordHandler.encode(passwordBox.Password, salt);

            userHandler.registerUser(usernameBox.Text, hashedPassword, salt);
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
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

            var loginSuccess = await userHandler.Login(usernameBox.Text, passwordBox.Password);
            if (loginSuccess == true)
            {
                HomeWindow home = new HomeWindow();
                home.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Warning: Incorrect Username Or Password");
            }
        }
    }
}
