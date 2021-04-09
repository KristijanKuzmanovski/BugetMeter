using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Firebase.Auth;
using Xamarin.Essentials;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;

namespace BudgetMeter
{
    public partial class App : Application
    {
       
        public static Database firebase;
        public App()
        {
            InitializeComponent();
            
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDIzMjA4QDMxMzkyZTMxMmUzMGp5NHp1aUtkeTB4SUJCMDBUUHl0emZTZkw3eWJGTFhITSsxcCtsVDZ4UEk9");
            firebase = new Database();
            MainPage = new NavigationPage(new Pages.LoginPage());
            
        }
        protected override void OnStart()
        {
            var chosen_theme = Preferences.Get("theme", "");

            if (string.IsNullOrWhiteSpace(chosen_theme))
            {
                var theme = Application.Current.RequestedTheme;

                switch (theme)
                {
                    case OSAppTheme.Light:
                        Application.Current.UserAppTheme = OSAppTheme.Light;
                        break;
                    case OSAppTheme.Dark:
                        Application.Current.UserAppTheme = OSAppTheme.Dark;
                        break;
                    default:
                        Application.Current.UserAppTheme = OSAppTheme.Light;
                        break;
                }
            }
            else
            {
                Console.WriteLine($"Theme:{chosen_theme}");
                switch (chosen_theme)
                {
                    case "Light":

                        Application.Current.UserAppTheme = OSAppTheme.Light;
                        break;
                    case "Dark":
                        Application.Current.UserAppTheme = OSAppTheme.Dark;
                        break;
                    default:
                        Application.Current.UserAppTheme = OSAppTheme.Light;
                        break;
                }
            }

        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public static async Task updateTokenAsync(FirebaseAuthLink res)
        {
            
            firebase.userLink = res;
            res.RefreshUserDetails().Wait();
            var content = (await res.GetFreshAuthAsync());
           
            var serilized_content = JsonConvert.SerializeObject(content);
            Preferences.Set("serilizedUser", serilized_content);


            firebase.currentUser = res.User;
            Preferences.Set("email", res.User.Email);
            await firebase.initRealtimeDatabaseAsync(res.FirebaseToken);
        }

        public static bool checkIfHasPin()
        {
            if (string.IsNullOrWhiteSpace(firebase.currentUserDetails.pin))
            {
                return false;
            }
            return true;
        }
        public static void logOut()
        {
            App.firebase.firestore = null;
            App.firebase.currentUser = null;
            App.firebase.userLink = null;
            App.firebase.currentUserDetails = null;
            Preferences.Set("serilizedUser", "");
            Application.Current.MainPage = new NavigationPage(new Pages.LoginPage());
        }

    }
}
