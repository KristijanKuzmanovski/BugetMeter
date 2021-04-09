using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BudgetMeter.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CategoryPage : ContentPage
    {
        
        public ObservableCollection<Models.Category> Items { get; set; }
        private List<Models.Category> list;
        private bool deleteEnabled;
        private Page mypage;
        private string typeC;
        public CategoryPage(bool delete = false, Page page = null, string _typeC = "")
        {
            InitializeComponent();
            deleteEnabled = delete;
            
            typeC = _typeC;
            if (!string.IsNullOrWhiteSpace(typeC))
            {
                type.IsVisible = false;
                if (typeC.Equals("expensess"))
                {
                    type.SelectedIndex = 0;
                }
                else
                {
                    type.SelectedIndex = 1;
                }
            }
            else
            {
                type.SelectedIndex = 0;
            }
            mypage = page;
            Items = new ObservableCollection<Models.Category>();
            
            MyListView.ItemsSource = Items;
            
        }
        public static String HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
       
        protected override void OnAppearing()
        {
            base.OnAppearing();
            setUp();
        }
        private void setUp()
        {
            if (type.SelectedIndex == 1)
            {
                populateList("income");
            }
            else if (type.SelectedIndex == 0)
            {
                populateList();
            }
        }
        private async void populateList(string type = "expense")
        {
            try
            {
                Items = new ObservableCollection<Models.Category>();
                list = await App.firebase.firestore.Child("categories").Child(App.firebase.currentUser.LocalId).Child(type).OnceSingleAsync<List<Models.Category>>();
                foreach(Models.Category x in list)
                {
                    Items.Add(x);
                }
                MyListView.ItemsSource = Items;

            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }        
        }
        async void Handle_ItemTapped(object sender, SelectionChangedEventArgs e)
        {
            Models.Category model = e.CurrentSelection.FirstOrDefault() as Models.Category;
            
            if (model == null)
            {
                ((CollectionView)sender).SelectedItem = null;
                return;
            }
            
            if (deleteEnabled)
            {
                bool flag = await DisplayAlert("Delete Category?", "Do you want to delete this category?", "Yes", "No");
                if (flag)
                {
                  

                        foreach (var a in list)
                        {
                            if (a.name.Equals(model.name))
                            {
                            Items.Remove(a);
                            list.Remove(a);
                                break;
                            }
                        }
                    if (type.SelectedIndex == 0)
                    {

                        await App.firebase.firestore.Child("categories").Child(App.firebase.currentUser.LocalId).Child("expense").PutAsync<List<Models.Category>>(list);
                        
                    }
                    else
                    {
                        await App.firebase.firestore.Child("categories").Child(App.firebase.currentUser.LocalId).Child("income").PutAsync<List<Models.Category>>(list);

                    }
                }
                
            }
            else if (mypage != null)
            {
                if (typeC.Equals("expensess"))
                {
                    var page = mypage as Pages.AddRecordPage;
                    page.chosen_category = model;
                    Navigation.PopAsync();
                }
                else
                {
                    var page = mypage as Pages.AddRecordPage;
                    page.chosen_category2 = model;
                    Navigation.PopAsync();
                }
               
            }
            else
            {
                ((CollectionView)sender).SelectedItem = null;
                return;
            }

            //Deselect Item
            ((CollectionView)sender).SelectedItem = null;
        }

        private void type_SelectedIndexChanged(object sender, EventArgs e)
        {
            Items = new ObservableCollection<Models.Category>();

            MyListView.ItemsSource = Items;
            if (type.SelectedIndex == 0)
            {
                populateList();
            }
            else
            {
                populateList("income");
            }
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (type.SelectedIndex == 0)
            {
                    Navigation.PushAsync(new Pages.AddCategoryPage(list.Count, "expense"));
            }
            else
            {
                Navigation.PushAsync(new Pages.AddCategoryPage(list.Count, "income"));
            }
           
        }
    }
}
