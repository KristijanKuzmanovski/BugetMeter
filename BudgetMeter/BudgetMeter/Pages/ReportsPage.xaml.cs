using Firebase.Database.Query;
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
    public partial class ReportsPage : ContentPage
    {
        public ReportsPage()
        {
            InitializeComponent();

            from.MinimumDate = App.firebase.currentUserDetails.created_at;
            to.MinimumDate = App.firebase.currentUserDetails.created_at;

            from.MaximumDate = DateTime.Now;
            to.MaximumDate = DateTime.Now;


            from.Date = DateTime.Parse($"01/{App.firebase.currentUserDetails.trackingMonth}/{DateTime.Now.Year}");
            to.Date = DateTime.Now;



            type.SelectedIndex = 0;

        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            string _type = type.SelectedItem.ToString();
            DateTime _from = from.Date; 
            DateTime _to = to.Date;
            List<Models.Record> list = new List<Models.Record>();
            error.IsVisible = false;

            if(_from.AddMonths(2) < _to){
                error.IsVisible = true;
                error.Text = "Date span needs to be smaller than two months.";
                return;
            }
            Console.WriteLine(_type);
            if (_type.Equals("All"))
            {
                for(var i = 0; i < 3; i++)
                {
                    var x = await App.firebase.firestore.Child("records").Child(App.firebase.currentUser.LocalId).Child(_from.Year.ToString()).Child((_from.Month + i).ToString()).OnceAsync<Models.Record>();
                    
                    foreach (var m in x)
                    {
                        
                        if (m.Object.date.Date.CompareTo(_from.Date) >= 0 && m.Object.date.Date.CompareTo(_to.Date) <= 0)
                        {
                            Console.WriteLine(m.Object.date);
                            list.Add(m.Object);
                        }
                    }
                }
            }
            else if (_type.Equals("Expense"))
            {
                for (var i = 0; i < 3; i++)
                {
                    var x = await App.firebase.firestore.Child("records").Child(App.firebase.currentUser.LocalId).Child(_from.Year.ToString()).Child((_from.Month + i).ToString()).OnceAsync<Models.Record>();

                    foreach (var m in x)
                    {
                        if (m.Object.date.CompareTo(_from) >= 0 && m.Object.date.CompareTo(_to) <= 0 && m.Object.type.Equals("expence"))
                        {
                            list.Add(m.Object);
                        }
                    }
                }
            }
            else if (_type.Equals("Income"))
            {
                for (var i = 0; i < 3; i++)
                {
                    var x = await App.firebase.firestore.Child("records").Child(App.firebase.currentUser.LocalId).Child(_from.Year.ToString()).Child((_from.Month + i).ToString()).OnceAsync<Models.Record>();

                    foreach (var m in x)
                    {
                        if (m.Object.date.CompareTo(_from) >= 0 && m.Object.date.CompareTo(_to) <= 0 && m.Object.type.Equals("income"))
                        {
                            list.Add(m.Object);
                        }
                    }
                }
            }
            string range = _from.Date.Date.ToString("dd/MM/yyyy") + " - " + _to.Date.Date.ToString("dd/MM/yyyy");
            Navigation.PushAsync(new Pages.ShowRecordsPage(list, range));

        }
    }
}