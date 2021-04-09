using Firebase.Database.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BudgetMeter.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddRecordPage : CarouselPage
    {
        public Models.Category chosen_category;
        public Models.Category chosen_category2;
        public AddRecordPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if(chosen_category == null)
            {
                clist.IsVisible = false;
            }
            else
            {
                clist.IsVisible = true;
                clist.Text = chosen_category.name;
                clist.TextColor = Color.FromHex(chosen_category.color);
            }

            if (chosen_category2 == null)
            {
                clist2.IsVisible = false;
            }
            else
            {
                clist2.IsVisible = true;
                clist2.Text = chosen_category2.name;
                clist2.TextColor = Color.FromHex(chosen_category2.color);
            }
        }
        private void goToExpense(object sender, EventArgs e)
        {
            if (carousel.TabIndex == 1)
            {
                carousel.TabIndex = 0;
                carousel.CurrentPage = one;
            }
        }
        private void goToIncome(object sender, EventArgs e)
        {
            if (carousel.TabIndex == 0)
            {
                carousel.TabIndex = 1;
                carousel.CurrentPage = two;
            }
        }

        private void add_category_Clicked(object sender, EventArgs e)
        {

            if (carousel.TabIndex == 0)
            {
                Navigation.PushAsync(new Pages.CategoryPage(false, this,"expensess"));
            }
            else
            {
                Navigation.PushAsync(new Pages.CategoryPage(false, this,"income"));
            }
           
        }

        private async void createExpence_Clicked(object sender, EventArgs e)
        {
            string _amount = amount.Text;
            string _at = at.Text;
            errors.IsVisible = false;

            if(string.IsNullOrWhiteSpace(_amount) || _amount.Equals("-") || _amount.Equals(".") || _amount.Equals(","))
            {
                errors.Text = "Invalid amount.";
                errors.IsVisible = true;
                return;
            }
            double amount_val = double.Parse(_amount);

            if(amount_val == 0)
            {
                errors.Text = "Invalid amount.";
                errors.IsVisible = true;
                return;
            }


            App.firebase.currentUserDetails.sum = App.firebase.currentUserDetails.sum - amount_val;
            App.firebase.currentUserDetails.expencess_sum = App.firebase.currentUserDetails.expencess_sum + amount_val;

            if(chosen_category == null)
            {
                chosen_category = new Models.Category() { color = "#121212", name = "other" };
            }

            string ud = JsonConvert.SerializeObject(App.firebase.currentUserDetails); 
            await App.firebase.firestore.Child("users").Child(App.firebase.currentUser.LocalId).PatchAsync(ud);
            string guid = Guid.NewGuid().ToString();
            await App.firebase.firestore.Child("records").Child(App.firebase.currentUser.LocalId).Child(DateTime.Now.Year.ToString()).Child(DateTime.Now.Month.ToString()).Child(guid).PutAsync<Models.Record>(new Models.Record()
            {
                id = guid,
                user_id = App.firebase.currentUser.LocalId,
                type = "expence",
                description = _at,
                amount = amount_val,
                category = chosen_category,
                date = DateTime.Now
            });
            Navigation.PopAsync();
        }

        private async void createIncome_Clicked(object sender, EventArgs e)
        {
            string _amount = amountI.Text;
            string _from = from.Text;
            errorsI.IsVisible = false;

            if (string.IsNullOrWhiteSpace(_amount) || _amount.Equals("-") || _amount.Equals(".") || _amount.Equals(","))
            {
                errorsI.Text = "Invalid amount.";
                errorsI.IsVisible = true;
                return;
            }
            double amount_val = double.Parse(_amount);

            if (amount_val == 0)
            {
                errorsI.Text = "Invalid amount.";
                errorsI.IsVisible = true;
                return;
            }


            App.firebase.currentUserDetails.sum = App.firebase.currentUserDetails.sum + amount_val;
            App.firebase.currentUserDetails.income_sum = App.firebase.currentUserDetails.income_sum + amount_val;

            if (chosen_category2 == null)
            {
                chosen_category2 = new Models.Category() { color = "#121212", name = "other" };
            }

            string ud = JsonConvert.SerializeObject(App.firebase.currentUserDetails);
            await App.firebase.firestore.Child("users").Child(App.firebase.currentUser.LocalId).PatchAsync(ud);
            string guid = Guid.NewGuid().ToString();
            
            await App.firebase.firestore.Child("records").Child(App.firebase.currentUser.LocalId).Child(DateTime.Now.Year.ToString()).Child(DateTime.Now.Month.ToString()).Child(guid).PutAsync<Models.Record>(new Models.Record()
            {
                id = guid,
                user_id = App.firebase.currentUser.LocalId,
                type = "income",
                description = _from,
                amount = amount_val,
                category = chosen_category2,
                date = DateTime.Now
            });
            Navigation.PopAsync();
        }
    }
}