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

    public partial class EditRecordPage : ContentPage
    {
        public Models.Category chosen_category;
        public Models.Category chosen_category2;
        Models.Record record;
        ShowRecordsPage page;
        public EditRecordPage(Models.Record _record, ShowRecordsPage _page)
        {
            InitializeComponent();
            page = _page as Pages.ShowRecordsPage;
            
            record = _record;
            amount.Text = record.amount.ToString();
            at.Text = record.description;
            if (!string.IsNullOrWhiteSpace(record.category.name))
            {
                clist.Text = record.category.name;
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (chosen_category == null)
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
                clist.IsVisible = false;
            }
            else
            {
                clist.IsVisible = true;
                clist.Text = chosen_category2.name;
                clist.TextColor = Color.FromHex(chosen_category2.color);
            }
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            await App.firebase.firestore.Child("records").Child(App.firebase.currentUser.LocalId)
                .Child(DateTime.Now.Year.ToString()).Child(DateTime.Now.Month.ToString()).Child(record.id).DeleteAsync();
            page.list.Remove(record);
            Navigation.PopAsync();
        }

        private void add_category_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Pages.CategoryPage(false, this, record.type));
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            string _amount = amount.Text;
            string _at = at.Text;
            errors.IsVisible = false;

            if (string.IsNullOrWhiteSpace(_amount) || _amount.Equals("-") || _amount.Equals(".") || _amount.Equals(","))
            {
                errors.Text = "Invalid amount.";
                errors.IsVisible = true;
                return;
            }
            double amount_val = double.Parse(_amount);

            if (amount_val == 0)
            {
                errors.Text = "Invalid amount.";
                errors.IsVisible = true;
                return;
            }
            page.list.Remove(record);
            App.firebase.currentUserDetails.sum = App.firebase.currentUserDetails.sum + (amount_val - record.amount);
            if (record.type.Equals("expence"))
            {
                App.firebase.currentUserDetails.expencess_sum = App.firebase.currentUserDetails.expencess_sum + (amount_val - record.amount);
            }
            else
            {
                App.firebase.currentUserDetails.income_sum = App.firebase.currentUserDetails.income_sum + (amount_val - record.amount);
            }
            record.amount = amount_val;
            record.description = _at;
            if(chosen_category != null)
            {
                record.category = chosen_category;
            }else if(chosen_category2 != null)
            {
                record.category = chosen_category2;
            }
            string ud = JsonConvert.SerializeObject(App.firebase.currentUserDetails);
            await App.firebase.firestore.Child("users").Child(App.firebase.currentUser.LocalId).PatchAsync(ud);
            
            await App.firebase.firestore.Child("records").Child(App.firebase.currentUser.LocalId).Child(DateTime.Now.Year.ToString()).Child(DateTime.Now.Month.ToString()).Child(record.id).PatchAsync<Models.Record>(record);
            page.list.Add(record);
            Navigation.PopAsync();
        }
    }
}