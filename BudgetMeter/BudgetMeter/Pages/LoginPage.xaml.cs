using Firebase.Auth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BudgetMeter.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
           
            checkIfUserIsLoggedIn();

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            string pastEmail = Preferences.Get("email", "");
            if (!string.IsNullOrWhiteSpace(pastEmail))
            {
                email.Text = pastEmail;
            }
        }
        private async void checkIfUserIsLoggedIn()
        {
            login_spinner.IsVisible = true;
            login_spinner.IsRunning = true;
            try
            {
                
                string serilizedUser = Preferences.Get("serilizedUser", "");
                Console.WriteLine($"USER: {serilizedUser}");
                if (!string.IsNullOrWhiteSpace(serilizedUser))
                {
                    FirebaseAuth unserilizedUser = JsonConvert.DeserializeObject<FirebaseAuth>(serilizedUser);
                    Console.WriteLine($"USE1R: {unserilizedUser.FirebaseToken}");
                    if (string.IsNullOrWhiteSpace(unserilizedUser.FirebaseToken))
                     {
                        login_spinner.IsVisible = false;
                        login_spinner.IsRunning = false;
                        return;
                     }

                    var res = await App.firebase.auth.RefreshAuthAsync(unserilizedUser);
                    
                    Console.WriteLine($"USE2R: {res.FirebaseToken}");
                    if (string.IsNullOrWhiteSpace(res.FirebaseToken))
                    {
                        login_spinner.IsVisible = false;
                        login_spinner.IsRunning = false;
                        return;
                    }

                    await App.updateTokenAsync(res);

                    login_spinner.IsVisible = false;
                    login_spinner.IsRunning = false;
                    
                    bool flag = App.checkIfHasPin();

                    if (flag)
                    {
                        await Navigation.PushModalAsync(new ModalPages.PinModalPage(false,true));
                    }
                    else
                    {
                        SlideMenu.SlideMenu menu = new SlideMenu.SlideMenu();
                        NavigationPage.SetHasNavigationBar(menu, false);
                        NavigationPage nav = new NavigationPage(menu);
                        Application.Current.MainPage = nav;
                    }
                    
                    

                }
                login_spinner.IsVisible = false;
                login_spinner.IsRunning = false;
            }
            catch (Exception ex)
            {
                login_spinner.IsVisible = false;
                login_spinner.IsRunning = false;
                Console.WriteLine(ex.Message);
            }

        }
        public async void LoginButton_Clicked(object sender, EventArgs e)
        {
            string _email = email.Text;
            string _password = password.Text;
            
            if (string.IsNullOrWhiteSpace(_email))
            {
                email.Placeholder = "Please enter your email.";
                email.Focus();
                return;
            }
            if (!IsValidEmail(_email))
            {
                email.Text = "";
                email.Placeholder = "Wrong email.";
                email.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(_password))
            {
                password.Text = "";
                password.Placeholder = "Please enter your password.";
                password.Focus();
                return;
            }

            login_spinner.IsVisible = true;
            login_spinner.IsRunning = true;
            error.IsVisible = false;

            try
            {
                var res = await App.firebase.auth.SignInWithEmailAndPasswordAsync(_email, _password);
                
               await App.updateTokenAsync(res);

                login_spinner.IsVisible = false;
                login_spinner.IsRunning = false;

                bool flag = App.checkIfHasPin();

                if (flag)
                {
                    await Navigation.PushModalAsync(new ModalPages.PinModalPage(false, true));
                }
                else
                {
                    SlideMenu.SlideMenu menu = new SlideMenu.SlideMenu();
                    NavigationPage.SetHasNavigationBar(menu, false);
                    NavigationPage nav = new NavigationPage(menu);

                    Application.Current.MainPage = nav;
                }
            }
            catch (FirebaseAuthException ex)
            {
                login_spinner.IsVisible = false;
                login_spinner.IsRunning = false;
                
                switch (ex.Reason.ToString())
                {
                    case "UnknownEmailAddress":
                        error.IsVisible = true;
                        error.Text = "Email does not exist.";
                        break;
                    case "WrongPassword":
                        error.IsVisible = true;
                        error.Text = "Wrong password.";
                        break;
                    default:
                        error.IsVisible = true;
                        error.Text = "An error has occured.";
                        break;
                }
                Console.WriteLine(ex);
            }
            catch(Exception _e)
            {
                login_spinner.IsVisible = false;
                login_spinner.IsRunning = false;
                error.IsVisible = true;
                error.Text = "An error has occured.";
                Console.WriteLine(_e);
            }
        }
        
       private void OnRegisterLabelTapped(object sender, EventArgs e)
        {
            
            Navigation.PushAsync(new Pages.RegisterPage());
        }
        public static bool IsValidEmail(string email)
        {
            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    }
}