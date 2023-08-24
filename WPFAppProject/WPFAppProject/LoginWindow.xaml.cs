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
        //Get the instance of password and user handler
        PasswordHandler passwordHandler = PasswordHandler.getInstance();
        UserHandler userHandler = UserHandler.getInstance();
        RegisterWindow register = null;

        //LoginWindow
        public LoginWindow()
        {
            InitializeComponent();
        }

        //On load
        private void LoginWindow_Load(object sender, EventArgs e)
        {
            //On load activities here

            //Get database access
            userHandler.accessDatabase();

            register = new RegisterWindow(this);
        }

        //On close
        private void CloseRelevant(object sender, EventArgs e)
        {
            register.Close();
        }

        //Set window minimum size
        private void Set_Minimums(object sender, EventArgs e)
        {
            //Set to actual size of content
            //Design view may look different to runtime
            //Thats why this exists
            MinWidth = ActualWidth;
            MinHeight = ActualHeight;
        }

        //Attempt Register
        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            register.Show();
            this.Hide();
        }

        //Attempt login
        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            //Warning if username textbox is empty 
            if (string.IsNullOrEmpty(usernameBox.Text))
            {
                MessageBox.Show("Warning: Enter Username");
                return;
            }

            //Warning if password password box is empty 
            if (string.IsNullOrEmpty(passwordBox.Password))
            {
                MessageBox.Show("Warning: Enter Password");
                return;
            }

            //Try username and password combination using userhandler
            var loginSuccess = await userHandler.Login(usernameBox.Text, passwordBox.Password);

            //If login is a success change windows
            if (loginSuccess == true)
            {
                HomeWindow home = new HomeWindow();
                home.Show();
                this.Close();
                register.Close();
            }
            else
            {
                MessageBox.Show("Warning: Incorrect Username Or Password");
            }
        }
    }
}
