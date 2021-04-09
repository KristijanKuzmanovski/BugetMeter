using Firebase.Auth;
using ImageCircle.Forms.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BudgetMeter.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserDetailsPage : ContentPage
    {
        bool flag = false;
        User user;
        public UserDetailsPage()
        {
            InitializeComponent();
            user = App.firebase.currentUser;
            username.Text = user.DisplayName;
            email.Text = user.Email;
            string _avatar = App.firebase.currentUser.PhotoUrl;
           
            if (!string.IsNullOrWhiteSpace(_avatar)){
                avatar.Source = ImageSource.FromUri(new Uri(_avatar));
            }
            else
            {
                avatar.Source = ImageSource.FromResource("BudgetMeter.Images.profile.png");
            }
            if (string.IsNullOrWhiteSpace(App.firebase.currentUserDetails.pin))
            {
                delete_pin.IsVisible = false;
            }

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (string.IsNullOrWhiteSpace(App.firebase.currentUserDetails.pin))
            {
                delete_pin.IsVisible = false;
            }
        }
        private void show_pass_form_Clicked(object sender, EventArgs e)
        {
            old_pass.IsVisible = !old_pass.IsVisible;
            pass.IsVisible = !pass.IsVisible;
            con_pass.IsVisible = !con_pass.IsVisible;
            flag = !flag;
        }

        private async void delete_account_Clicked(object sender, EventArgs e)
        {
            bool flag = await DisplayAlert("Delete Account?", "Do you want to delete your account?", "Yes", "No");
            if (flag)
            {
                string result = await DisplayPromptAsync("Delete account", "Please enter your password.");
                
                try
                {
                    var _res = await App.firebase.auth.SignInWithEmailAndPasswordAsync(email.Text, result);
                    App.firebase.auth.DeleteUser(_res.FirebaseToken);
                    Preferences.Set("email", "");
                    App.logOut();
                }
                catch (FirebaseAuthException ex)
                {
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
                }
                
            }
            
        }
        private void goBack()
        {
            Navigation.PopAsync();
        }
        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            error.IsVisible = false;

            string _username = username.Text;
            try
            {
                if (!string.IsNullOrEmpty(_username) && !_username.Equals(App.firebase.currentUser.DisplayName))
                {
                    //var res = await App.firebase.userLink.UpdateProfileAsync(_username, "");
                    var res = await App.firebase.auth.UpdateProfileAsync(App.firebase.userLink.FirebaseToken, _username, App.firebase.currentUser.PhotoUrl);
                    
                   
                    App.firebase.currentUser = res.User;
                    //var page = Application.Current.MainPage as SlideMenu.SlideMenu;
                    //page.changeName(_username);
                    goBack();
                }
                if (flag)
                {
                    Console.WriteLine($"Flag is {flag}");
                    string _old_pass = old_pass.Text;
                    string _pass = pass.Text;
                    string _con_pass = con_pass.Text;

                    try
                    {
                        if (string.IsNullOrWhiteSpace(_old_pass))
                        {
                            old_pass.Placeholder = "Please enter your password.";
                            old_pass.Focus();
                            return;
                        }
                        if (_old_pass.Length < 6)
                        {
                            old_pass.Text = "";
                            old_pass.Placeholder = "Password too short.";
                            old_pass.Focus();
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
                        var _res = await App.firebase.auth.SignInWithEmailAndPasswordAsync(email.Text, _old_pass);
                        var res2 = await App.firebase.auth.ChangeUserPassword(App.firebase.userLink.FirebaseToken, _pass);

                        App.logOut();
                    }
                    catch (FirebaseAuthException ex)
                    {
                        Console.WriteLine(ex);
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
                    }
                }
            }
            catch (Exception er)
            {
                error.IsVisible = true;
                error.Text = "An error has occured.";
                Console.WriteLine(er);
            }
            goBack();
        }
        async void TakePhotoAsync()
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();

                var stream = await photo.OpenReadAsync();

                avatar.Source = ImageSource.FromStream(() => stream);

                var img = App.firebase.storage.Child("images").Child(App.firebase.currentUser.LocalId+".png").PutAsync(await photo.OpenReadAsync());
                img.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");
                var img_url = await img;

                var res = await App.firebase.auth.UpdateProfileAsync(App.firebase.userLink.FirebaseToken, App.firebase.userLink.User.DisplayName, img_url);
                
                App.firebase.currentUser = res.User;
                var page = Application.Current.MainPage.Navigation.NavigationStack.Last() as SlideMenu.SlideMenu;
                page.refresh();
                
                Console.WriteLine($"CapturePhotoAsync COMPLETED: {img_url}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CapturePhotoAsync THREW: {ex}");
            }
        }
        async void PickPhotoAsync()
        {
            try
            {
                var photo = await MediaPicker.PickPhotoAsync();

                var stream = await photo.OpenReadAsync();

                avatar.Source = ImageSource.FromStream(() => stream);

                var img = App.firebase.storage.Child("images").Child(App.firebase.currentUser.LocalId + ".png").PutAsync(await photo.OpenReadAsync());
                img.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");
                var img_url = await img;

                var res = await App.firebase.auth.UpdateProfileAsync(App.firebase.userLink.FirebaseToken, App.firebase.userLink.User.DisplayName, img_url);

                App.firebase.currentUser = res.User;

                var page = Application.Current.MainPage.Navigation.NavigationStack.Last() as SlideMenu.SlideMenu;
                page.refresh();

                Console.WriteLine($"CapturePhotoAsync COMPLETED: {img_url}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CapturePhotoAsync THREW: {ex}");
            }
        }
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Choose photo location.", "Cancel", null, "Camera", "Gallery", "Delete profile image");
            switch (action)
            {
                case "Camera":
                    TakePhotoAsync();
                    break;
                case "Gallery":
                    PickPhotoAsync();
                    break;
                case "Delete profile image":
                    avatar.Source = ImageSource.FromResource("BudgetMeter.Images.profile.png");
                    var img = App.firebase.storage.Child("images").Child(App.firebase.currentUser.LocalId + ".png").DeleteAsync();
                    var res = await App.firebase.auth.UpdateProfileAsync(App.firebase.userLink.FirebaseToken, App.firebase.userLink.User.DisplayName, "");

                    App.firebase.currentUser = res.User;
                    break;
                default:
                    break;
            }
        }

        private async void delete_pin_ClickedAsync(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ModalPages.PinModalPage(false, false, true,true));
        }
    }
    
}