using Firebase.Database.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BudgetMeter.ModalPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PinModalPage : ContentPage
    {
        Stack<int> f;
        string firstPin;
        bool isPinBeingCreated;
        bool isLoginPage;
        bool isAsking;
        bool isDeleting;
        bool result;
        string pin;
        public PinModalPage(bool flag = false, bool isLogin = false, bool ask=false, bool delete = false)
        {
            InitializeComponent();
            isPinBeingCreated = flag;
            isLoginPage = isLogin;
            isAsking = ask;
            isDeleting = delete;

            if (string.IsNullOrWhiteSpace(App.firebase.currentUserDetails.pin) && !isPinBeingCreated)
            {
                Navigation.PopModalAsync();
            }
            if (isAsking)
            {
                result = false;
            }
            if (!isPinBeingCreated)
            {
                pin = App.firebase.currentUserDetails.pin;  
            }
            else
            {
                header.Text = "Create pin.";
            }
           
            f = new Stack<int>();
            firstPin = "";
        }

        private void PinOne_Tapped(object sender, EventArgs e)
        {
            if (f.Count < 4)
            {
                f.Push(1);
                UpdateVisual_Passcode();
            }
            

            
        }

        private void PinTwo_Tapped(object sender, EventArgs e)
        {
            if (f.Count < 4)
            {
                f.Push(2);
                UpdateVisual_Passcode();
            }
            

            
        }

        private void PinThree_Tapped(object sender, EventArgs e)
        {
            if (f.Count < 4)
            {
                f.Push(3);
                UpdateVisual_Passcode();
            }
            

            
        }

        private void PinFour_Tapped(object sender, EventArgs e)
        {
            if (f.Count < 4)
            {
                f.Push(4);
                UpdateVisual_Passcode();
            }
             ;

           
        }

        private void PinFive_Tapped(object sender, EventArgs e)
        {
            if (f.Count < 4)
            {
                f.Push(5);
                UpdateVisual_Passcode();
            }
             ;

           
        }

        private void PinSix_Tapped(object sender, EventArgs e)
        {
            if (f.Count < 4)
            {
                f.Push(6);
                UpdateVisual_Passcode();
            }
            

            
        }

        private void PinSeven_Tapped(object sender, EventArgs e)
        {
            if (f.Count < 4)
            {
                f.Push(7);
                UpdateVisual_Passcode();
            }
            

            
        }

        private void PinEight_Tapped(object sender, EventArgs e)
        {
            if (f.Count < 4)
            {
                f.Push(8);
                UpdateVisual_Passcode();
            }
            

            
        }

        private void PinNine_Tapped(object sender, EventArgs e)
        {
            if (f.Count < 4)
            {
                f.Push(9);
                UpdateVisual_Passcode();
            }
             ;

            
        }

        private void PinZero_Tapped(object sender, EventArgs e)
        {
            if (f.Count < 4)
            {
                f.Push(0);
                UpdateVisual_Passcode();
            }
            

            
        }

        private void PinDelete_Tapped(object sender, EventArgs e)
        {
            if (f.Count == 0)
                return;
            f.Pop();           

            UpdateVisual_Passcode();
        }

        private void UpdateVisual_Passcode()
        {
            switch (f.Count)
            {
                case 0:
                    PasscodeOne.Fill= new SolidColorBrush(Color.FromHex("#8F8F8F"));
                    PasscodeTwo.Fill= new SolidColorBrush(Color.FromHex("#8F8F8F"));
                    PasscodeThree.Fill= new SolidColorBrush(Color.FromHex("#8F8F8F"));
                    PasscodeFour.Fill= new SolidColorBrush(Color.FromHex("#8F8F8F"));
                    break;
                case 1:
                    PasscodeOne.Fill= new SolidColorBrush(Color.FromHex("#4549D1"));
                    PasscodeTwo.Fill= new SolidColorBrush(Color.FromHex("#8F8F8F"));
                    PasscodeThree.Fill= new SolidColorBrush(Color.FromHex("#8F8F8F"));
                    PasscodeFour.Fill= new SolidColorBrush(Color.FromHex("#8F8F8F"));
                    break;
                case 2:
                    PasscodeOne.Fill= new SolidColorBrush(Color.FromHex("#4549D1"));
                    PasscodeTwo.Fill= new SolidColorBrush(Color.FromHex("#4549D1"));
                    PasscodeThree.Fill= new SolidColorBrush(Color.FromHex("#8F8F8F"));
                    PasscodeFour.Fill= new SolidColorBrush(Color.FromHex("#8F8F8F"));
                    break;
                case 3:
                    PasscodeOne.Fill= new SolidColorBrush(Color.FromHex("#4549D1"));
                    PasscodeTwo.Fill= new SolidColorBrush(Color.FromHex("#4549D1"));
                    PasscodeThree.Fill= new SolidColorBrush(Color.FromHex("#4549D1"));
                    PasscodeFour.Fill= new SolidColorBrush(Color.FromHex("#8F8F8F"));
                    break;
                case 4:
                    PasscodeOne.Fill= new SolidColorBrush(Color.FromHex("#4549D1"));
                    PasscodeTwo.Fill= new SolidColorBrush(Color.FromHex("#4549D1"));
                    PasscodeThree.Fill= new SolidColorBrush(Color.FromHex("#4549D1"));
                    PasscodeFour.Fill= new SolidColorBrush(Color.FromHex("#4549D1"));
                    ConfirmPinCodeAsync();
                    break;
            }
        }

        private async Task ConfirmPinCodeAsync()
        {
            string entered_pin = "";
            foreach(int num in f.Reverse())
            {
                entered_pin += num;
            }
            Console.WriteLine(entered_pin);

            if (isPinBeingCreated)
            {
                if(!string.IsNullOrWhiteSpace(firstPin))
                {
                    
                    if (entered_pin.Equals(firstPin))
                    {
                        
                        App.firebase.currentUserDetails.pin = entered_pin;
                        string usr = JsonConvert.SerializeObject(App.firebase.currentUserDetails);
                        App.firebase.firestore.Child("users").Child(App.firebase.currentUser.LocalId).PatchAsync(usr);
                        
                        logIn();
                    }
                    else
                    {
                        clearPin();
                        firstPin = "";
                        header.Text = "Pins don't match.";
                        header.TextColor = Color.Red;
                    }
                }
                else
                {
                    firstPin = entered_pin;
                    clearPin();
                    header.Text = "Confirm pin.";
                    header.TextColor = Color.Blue;
                }

            }
            else
            {
                if (entered_pin.Equals(pin))
                {
                    logIn();
                }
                else
                {
                    header.Text = "Wrong pin.";
                    header.TextColor = Color.Red;
                    clearPin();
                }
            }
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if(isDeleting && result && isAsking)
            {
                App.firebase.currentUserDetails.pin = "";
                string usr = JsonConvert.SerializeObject(App.firebase.currentUserDetails);
                App.firebase.firestore.Child("users").Child(App.firebase.currentUser.LocalId).PatchAsync(usr);

                return;
            }
            if (result && isAsking)
            {
                Navigation.PushModalAsync(new ModalPages.PinModalPage(true));
            }
        }
        private void  logIn()
        {
            if (isLoginPage)
            {
                SlideMenu.SlideMenu menu = new SlideMenu.SlideMenu();
                NavigationPage.SetHasNavigationBar(menu, false);
                NavigationPage nav = new NavigationPage(menu);

                Application.Current.MainPage = nav;
            }
            else
            {
                result = true;
                Navigation.PopModalAsync();
            }
        }
        private void clearPin()
        {
            f.Clear();
            PasscodeOne.Fill= new SolidColorBrush(Color.FromHex("#8F8F8F"));
            PasscodeTwo.Fill= new SolidColorBrush(Color.FromHex("#8F8F8F"));
            PasscodeThree.Fill= new SolidColorBrush(Color.FromHex("#8F8F8F"));
            PasscodeFour.Fill= new SolidColorBrush(Color.FromHex("#8F8F8F"));
        }

        protected override bool OnBackButtonPressed()
        {
            if (isAsking)
            {
                result = false;
                Navigation.PopModalAsync();
            }
            if (isPinBeingCreated)
            {
                Navigation.PopModalAsync();
            }
            return true;
        }
    }
}