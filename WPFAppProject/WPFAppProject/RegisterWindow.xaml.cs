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
using System.Windows.Shapes;
using WPFAppProject.Data;

namespace WPFAppProject
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        //Get the instance of password and user handler
        PasswordHandler passwordHandler = PasswordHandler.getInstance();
        UserHandler userHandler = UserHandler.getInstance();
        Window parentWindow = null;

        //LoginWindow
        public RegisterWindow(Window parent)
        {
            InitializeComponent();
            parentWindow = parent;
        }

        //On load
        private void RegisterWindow_Load(object sender, EventArgs e)
        {
            //On load activities here
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
            //Warning if no username in username text box
            if (string.IsNullOrEmpty(usernameBox.Text))
            {
                MessageBox.Show("Warning: Username Required");
                return;
            }

            //Warning if no password in password password box
            if (string.IsNullOrEmpty(passwordBox.Password))
            {
                MessageBox.Show("Warning: Password Required");
                return;
            }

            //Warning if username is less than 5 characters in length
            if (usernameBox.Text.Length < 5)
            {
                MessageBox.Show("Warning: Username Must Be 5 Or More Characters In Length");
                return;
            }

            //Warning if username is more than 20 characters in length
            if (usernameBox.Text.Length > 20)
            {
                MessageBox.Show("Warning: Username Must Be 20 Or Less Characters In Length");
                return;
            }

            ////Warning if username contains a space
            if (usernameBox.Text.Contains(" "))
            {
                MessageBox.Show("Warning: Username Cannot Contain Spaces");
                return;
            }

            //Generate some salt for the hashed password
            byte[] salt = passwordHandler.GenerateSalt();
            //Generate a hashed password in password handler by giving plain text password and the salt
            string hashedPassword = passwordHandler.encode(passwordBox.Password, salt);

            //Handle registering the user in userhandler, send over username, encrypted password and salt
            userHandler.registerUser(usernameBox.Text, hashedPassword, salt);
        }

        //Attempt login
        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            parentWindow.Show();
        }
    }
}
