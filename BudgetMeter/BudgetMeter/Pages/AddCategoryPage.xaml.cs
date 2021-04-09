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
    public partial class AddCategoryPage : ContentPage
    {
        private int ID;
        private string type;
        public AddCategoryPage(int id,string _type)
        {
            InitializeComponent();
            ID = id;
            type = _type;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            string _color = "";
            string _name = name.Text;
            if (string.IsNullOrWhiteSpace(_name))
            {
                name.Focus();
                name.Placeholder = "Plesae enter a name.";
                return;
            }

            _color = PopulateCategories.HexConverter(ColorWheel1.SelectedColor);

            await App.firebase.firestore.Child("categories").Child(App.firebase.currentUser.LocalId).Child(type).Child(ID.ToString()).PutAsync( new Models.Category() { name = _name, color = _color  });
            Console.WriteLine(ColorWheel1.SelectedColor);

            Navigation.PopAsync();
        }
    }
}