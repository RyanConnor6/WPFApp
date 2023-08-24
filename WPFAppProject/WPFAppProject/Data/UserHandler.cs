using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using FirebaseAdmin.Messaging;
using System.Collections;
using Google.Cloud.Firestore.V1;

namespace WPFAppProject.Data
{
    public sealed class UserHandler
    {
        //Singelton code
        private static UserHandler reference;
        private FirestoreDb db;
        PasswordHandler passwordHandler = PasswordHandler.getInstance();

        //Constructor, empty as there should only be one
        public UserHandler() { }

        //Database getters

        //Get database
        public FirestoreDb Db
        {
            get { return db; }
        }

        //Create the only instance of handler
        public static UserHandler getInstance()
        {
            if (reference == null)
                reference = new UserHandler();

            return reference;
        }

        //Access the database
        public void accessDatabase()
        {
            //Get database access, access json from database
            string path = AppDomain.CurrentDomain.BaseDirectory + @"wpfappdatabaseproject-firebase-adminsdk-7qfur-59c54e3204.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            db = FirestoreDb.Create("wpfappdatabaseproject");
        }

        //Register a user
        public void registerUser(string desiredName, string hashedPassword, byte[] salt)
        {
            //Get reference to database userLogin collection
            CollectionReference coll = db.Collection("userLogin");

            //Send all relevant data to a dictionary
            Dictionary<string, object> data1 = new Dictionary<string, object>()
            {
                {"Username", desiredName},
                {"Password", hashedPassword},
                {"Salt", Convert.ToBase64String(salt)}
            };
            coll.AddAsync(data1);
        }

        //Login a user
        public async Task<bool> Login(string desiredName, string desiredPassword)
        {
            //Try logging in
            try
            {
                //Setting up variables
                var attemptedPassword = "a";
                var attemptedSalt = "a";

                //Create a query to find the user with the input username
                Query query = db.Collection("userLogin").WhereEqualTo("Username", desiredName);
                var queryNameTask = query.GetSnapshotAsync();
                while (!queryNameTask.IsCompleted) await Task.Yield();

                //Store query result
                var querySnapshot = queryNameTask.Result;

                //For each result, change into a dictionary and get salt and password from database
                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                {
                    Dictionary<string, object> stuff = documentSnapshot.ToDictionary();
                    foreach (KeyValuePair<string, object> kvp in stuff)
                    {
                        //attemptedSalt is salt found
                        if (kvp.Key.Equals("Salt"))
                        {
                            attemptedSalt = kvp.Value.ToString();
                        }
                        //attemptedPassword is hashed password found
                        if (kvp.Key.Equals("Password"))
                        {
                            attemptedPassword = kvp.Value.ToString();
                        }
                    }
                }

                //If the query has no documents, return a fail
                if (querySnapshot.Documents.Count == 0)
                {
                    return false;
                }
                //Otherwise encode password provided using salt and verify if it matches the hashed password from the database 
                else
                {
                    string encoded = passwordHandler.encode(desiredPassword, Convert.FromBase64String(attemptedSalt));
                    //If they match return a success
                    if (encoded.Equals(attemptedPassword))
                    {
                        return true;
                    }
                    //Otherwise fail
                    else
                    {
                        return false;
                    }
                }
            }

            //Cant do something in the operation, return a fail
            catch (Exception e)
            {
                MessageBox.Show("I'm here now!");
                return false;
            }
        }
    }
}
