using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BudgetMeter.ModalPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IncomeInfoModalPage : ContentPage
    {
        bool register;
        public IncomeInfoModalPage(bool isRegister = true)
        {
            InitializeComponent();
            currency.SelectedIndex = 0;
            register = isRegister;
        }


        protected override bool OnBackButtonPressed()
        {
            return true;
        }
        private async void CompleteButton_Clicked(object sender, EventArgs e)
        {
            var _income = income.Text;
            var _currency = currency.SelectedItem;

            if (string.IsNullOrWhiteSpace(_income) || _income.Equals("-") || _income.Equals(".") || _income.Equals(","))
            {
                income.Placeholder = "income can't be zero.";
                income.Focus();
                return;
            }
            if (register)
            {
               await App.firebase.firestore.Child("users").Child(App.firebase.currentUser.LocalId).PutAsync(new Models.UserDetails()
                {
                    sum = int.Parse(_income),
                    currency = _currency.ToString(),
                    pin = "",
                    salary = int.Parse(_income),
                    expencess_sum = 0,
                    income_sum = 0,
                    trackingMonth = DateTime.Now.Month,
                    created_at = DateTime.Now
                });
                Navigation.PopModalAsync();
            }
            else
            {
                App.firebase.currentUserDetails.salary = int.Parse(_income);
                App.firebase.currentUserDetails.currency = _currency.ToString();
                await App.firebase.firestore.Child("users").Child(App.firebase.currentUser.LocalId).PatchAsync(App.firebase.currentUserDetails);
                Navigation.PopAsync();
            }
           
        }
    }
}