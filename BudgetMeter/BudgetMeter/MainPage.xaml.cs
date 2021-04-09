using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BudgetMeter
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
             //App.firebase.firestore.Child("categories").Child(App.firebase.currentUser.LocalId).Child("expense").PutAsync<List<Models.Category>>(PopulateCategories.defaultExpenceCategories);
             //App.firebase.firestore.Child("categories").Child(App.firebase.currentUser.LocalId).Child("income").PutAsync<List<Models.Category>>(PopulateCategories.defaultIncomeCategories);

            if(App.firebase.currentUserDetails.trackingMonth != DateTime.Now.Month)
            {
                App.firebase.currentUserDetails.trackingMonth = DateTime.Now.Month;
                App.firebase.currentUserDetails.expencess_sum = 0;
                App.firebase.currentUserDetails.expencess_sum = 0;
                App.firebase.currentUserDetails.sum = App.firebase.currentUserDetails.salary;
            }
            main_chart.Legend = new Syncfusion.SfChart.XForms.ChartLegend();
            main_chart.Title.Text = "This month";
           
        }



        protected override void OnAppearing()
        {
            base.OnAppearing();

            updateUI();
        }
        private async void updateUI()
        {
            total.Text = $"{App.firebase.currentUserDetails.sum}{App.firebase.currentUserDetails.currency}";
            plus.Text = $"+{App.firebase.currentUserDetails.income_sum}{App.firebase.currentUserDetails.currency}";
            if(App.firebase.currentUserDetails.expencess_sum < 0)
            {
                minus.Text = $"{App.firebase.currentUserDetails.expencess_sum}{App.firebase.currentUserDetails.currency}";
            }
            else
            {
                minus.Text = $"-{App.firebase.currentUserDetails.expencess_sum}{App.firebase.currentUserDetails.currency}";
            }
           ;
            var tmp = await App.firebase.firestore.Child("records").Child(App.firebase.currentUser.LocalId).Child(DateTime.Now.Year.ToString()).Child(App.firebase.currentUserDetails.trackingMonth.ToString()).OnceAsync<Models.Record>();
            List<Models.Record> expenses_list = new List<Models.Record>();
            List<Models.Record> income_list = new List<Models.Record>();
            foreach(var r in tmp)
            {
                if (r.Object.type.Equals("income"))
                {
                    income_list.Add(r.Object);
                }
                else
                {
                    expenses_list.Add(r.Object);
                }
            }
            IEnumerable<Models.Record> query =income_list.OrderBy(r => r.date);
            IEnumerable<Models.Record> query2 =expenses_list.OrderBy(r => r.date);
            try
            {
                chart.ItemsSource = new ObservableCollection<Models.Record>(query2);
                chart2.ItemsSource = new ObservableCollection<Models.Record>(query);
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
         
        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Pages.AddRecordPage());
        }
    }
}
