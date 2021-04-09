using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;

namespace BudgetMeter
{
    public class Database
    {
        public FirebaseClient firestore;
        public FirebaseAuthProvider auth;
        public User currentUser;
        public FirebaseAuthLink userLink;
        public FirebaseStorage storage;
        public Models.UserDetails currentUserDetails;

        public Database()
        {
            auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyAOQCvG7okIoSzuko4cf8jAus6rNYsgWXk"));
        }

        public async Task initRealtimeDatabaseAsync(string token)
        {
            firestore = new FirebaseClient("https://budgetmeter-2a1f3-default-rtdb.firebaseio.com/", new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token)});
            Console.WriteLine(firestore);
            storage = new FirebaseStorage("budgetmeter-2a1f3.appspot.com");
            currentUserDetails = await firestore.Child("users").Child(currentUser.LocalId).OnceSingleAsync<Models.UserDetails>();
            Console.WriteLine($"USER: {currentUserDetails.pin}");
        }
        public async Task initRealtimeDatabaseAsyncForRegister(string token)
        {
            firestore = new FirebaseClient("https://budgetmeter-2a1f3-default-rtdb.firebaseio.com/", new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token) });
            Console.WriteLine(firestore);
            storage = new FirebaseStorage("budgetmeter-2a1f3.appspot.com");
        }
    }
}
