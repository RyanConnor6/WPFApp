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

        public UserHandler() { }

        //Database getter
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

        public void accessDatabase()
        {
            //Get database access, access json from database
            string path = AppDomain.CurrentDomain.BaseDirectory + @"wpfappdatabaseproject-firebase-adminsdk-7qfur-59c54e3204.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            db = FirestoreDb.Create("wpfappdatabaseproject");
        }

        public void registerUser(string desiredName, string hashedPassword, byte[] salt)
        {
            CollectionReference coll = db.Collection("userLogin");
            Dictionary<string, object> data1 = new Dictionary<string, object>()
            {
                {"Username", desiredName},
                {"Password", hashedPassword},
                {"Salt", Convert.ToBase64String(salt)}
            };
            coll.AddAsync(data1);
        }

        public async Task endQuery()
        {

        }

        public async Task<bool> Login(string desiredName, string desiredPassword)
        {
            try
            {
                var attemptedPassword = "a";
                var attemptedSalt = "a";
                Query query = db.Collection("userLogin").WhereEqualTo("Username", desiredName);
                var queryNameTask = query.GetSnapshotAsync();
                while (!queryNameTask.IsCompleted) await Task.Yield();

                var querySnapshot = queryNameTask.Result;

                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                {
                    Dictionary<string, object> stuff = documentSnapshot.ToDictionary();
                    foreach (KeyValuePair<string, object> kvp in stuff)
                    {
                        if (kvp.Key.Equals("Salt"))
                        {
                            attemptedSalt = kvp.Value.ToString();
                        }
                        if (kvp.Key.Equals("Password"))
                        {
                            attemptedPassword = kvp.Value.ToString();
                        }
                    }
                }

                if (querySnapshot.Documents.Count == 0)
                {
                    return false;
                }
                else
                {
                    string encoded = passwordHandler.encode(desiredPassword, Convert.FromBase64String(attemptedSalt));
                    if (encoded.Equals(attemptedPassword))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            catch (Exception e)
            {
                MessageBox.Show("I'm here now!");
                return false;
            }

            /*
            Google.Cloud.Firestore.DocumentReference docref = db.Collection("userLogin").Document(desiredName);
            DocumentSnapshot snap = await docref.GetSnapshotAsync();

            var attemptedPassword = "a";
            var attemptedSalt = "a";

            if (snap.Exists)
            {
                Dictionary<string, object> user = snap.ToDictionary();
                foreach (var item in user)
                {
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
            else
            {
                return false;
            }

            string encoded = passwordHandler.encode(desiredPassword, Convert.FromBase64String(attemptedSalt));

            if (encoded.Equals(attemptedPassword))
            {
                return true;
            }
            else
            {
                return false;
            }
            */
        }
    }
}
