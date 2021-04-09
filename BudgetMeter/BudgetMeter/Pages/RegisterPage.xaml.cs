using Firebase.Auth;
using Firebase.Database.Query;
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
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
          
        }
        private async void RegisterButton_Clicked(object sender, EventArgs e)
        {
            string _username = username.Text;
            string _email = email.Text;
            string _pass = pass.Text;
            string _con_pass = con_pass.Text;

            if (string.IsNullOrWhiteSpace(_username))
            {
                username.Placeholder = "Please enter your display name.";
                username.Focus();
                return;
            }
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
            if (string.IsNullOrWhiteSpace(_pass))
            {
                pass.Placeholder = "Please enter your password.";
                pass.Focus();
                return;
            }
            if (_pass.Length < 6)
            {
                pass.Text = "";
                pass.Placeholder = "Password too short.";
                pass.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(_con_pass))
            {
                con_pass.Placeholder = "Please enter your confirmation password.";
                con_pass.Focus();
                return;
            }

            if (!_pass.Equals(_con_pass))
            {
                con_pass.Text = "";
                con_pass.Placeholder = "Passwords don't match.";
                con_pass.Focus();
                return;
            }
            register_spinner.IsVisible= true;
            register_spinner.IsRunning = true;

            try
            {
                error.IsVisible = false;
                var res = await App.firebase.auth.CreateUserWithEmailAndPasswordAsync(_email, _pass,_username);
                

                App.firebase.userLink = res;
                App.firebase.currentUser = res.User;
                Preferences.Set("email", res.User.Email);
                
                await App.firebase.initRealtimeDatabaseAsyncForRegister(res.FirebaseToken);
                await App.firebase.firestore.Child("categories").Child(res.User.LocalId).Child("expense").PutAsync<List<Models.Category>>(PopulateCategories.defaultExpenceCategories);
                await App.firebase.firestore.Child("categories").Child(res.User.LocalId).Child("income").PutAsync<List<Models.Category>>(PopulateCategories.defaultIncomeCategories);

                register_spinner.IsVisible = false;
                register_spinner.IsRunning = false;
                await Navigation.PushModalAsync(new ModalPages.IncomeInfoModalPage());

                

                await Navigation.PopToRootAsync();
                
            }
            catch(FirebaseAuthException ep)
            {
                register_spinner.IsVisible = false;
                register_spinner.IsRunning = false;

                switch (ep.Reason.ToString())
                {
                    case "EmailExists":
                        email.Focus();
                        error.IsVisible = true;
                        error.Text = "Email is in use.";
                        break;
                    default:
                        break;
                }
                Console.WriteLine(ep.Message);
            }
            catch(Exception ex)
            {
                register_spinner.IsVisible = false;
                register_spinner.IsRunning = false;
                Console.WriteLine(ex.Message);
            }
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