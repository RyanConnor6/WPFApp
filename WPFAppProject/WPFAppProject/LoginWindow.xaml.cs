﻿using System;
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
        FirestoreDb db;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginWindow_Load(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"wpfappdatabaseproject-firebase-adminsdk-7qfur-59c54e3204.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            db = FirestoreDb.Create("wpfappdatabaseproject");
            MessageBox.Show("Successful");
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

            /*
            using StreamWriter file = new($"TestPassword.txt");
            file.WriteLine(usernameBox.Text);
            file.WriteLine(Convert.ToBase64String(salt));
            file.WriteLine(hashedPassword);
            */

            Google.Cloud.Firestore.DocumentReference doc = db.Collection("userLogin").Document(usernameBox.Text);
            Dictionary<string, object> data1 = new Dictionary<string, object>()
            {
                {"Password", hashedPassword},
                {"Salt", Convert.ToBase64String(salt)}
            };
            doc.SetAsync(data1);
            MessageBox.Show("Data added");
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

            /*
            StreamReader sr = new StreamReader($"TestPassword.txt");
            var line = sr.ReadLine();
            line = sr.ReadLine();
            var salt = Convert.FromBase64String(line);
            line = sr.ReadLine();
            var hashedPassword = line;
            */

            Google.Cloud.Firestore.DocumentReference docref = db.Collection("userLogin").Document(usernameBox.Text);
            DocumentSnapshot snap = await docref.GetSnapshotAsync();

            var attemptedPassword = "a";
            var attemptedSalt = "a";

            if (snap.Exists)
            {
                Dictionary<string, object> user = snap.ToDictionary();
                foreach (var item in user){
                    if (item.Key.Equals("Password"))
                    {
                        attemptedPassword = item.Value.ToString();
                    }
                    if (item.Key.Equals("Salt"))
                    {
                        attemptedSalt = item.Value.ToString();
                    }
                }
            }

            string encoded = passwordHandler.encode(passwordBox.Password, Convert.FromBase64String(attemptedSalt));

            if (encoded.Equals(attemptedPassword))
            {
                MessageBox.Show("Correct Password");
                HomeWindow home = new HomeWindow();
                home.Show();
                this.Close();
            }

            /*
            string encoded = passwordHandler.encode(passwordBox.Password, salt);

            if (encoded.Equals(hashedPassword))
            {
                MessageBox.Show("Correct Password");
                HomeWindow home = new HomeWindow();
                home.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Incorrect Password");
            }

            sr.Close();
            */
        }
    }
}
