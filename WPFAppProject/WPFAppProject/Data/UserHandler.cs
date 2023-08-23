using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;

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
            Google.Cloud.Firestore.DocumentReference doc = db.Collection("userLogin").Document(desiredName);
            Dictionary<string, object> data1 = new Dictionary<string, object>()
            {
                {"Password", hashedPassword},
                {"Salt", Convert.ToBase64String(salt)}
            };
            doc.SetAsync(data1);
        }

        public async Task<bool> Login(string desiredName, string desiredPassword)
        {
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
        }
    }
}
