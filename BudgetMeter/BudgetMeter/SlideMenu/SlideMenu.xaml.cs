using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BudgetMeter.SlideMenu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SlideMenu : FlyoutPage
    {
        public SlideMenu()
        {
            InitializeComponent();
            FlyoutPage.ListView.ItemSelected += ListView_ItemSelectedAsync;
            
        }
        public  void refresh()
        {
            
            FlyoutPage.checkUserDetails();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            FlyoutPage.checkUserDetails();
        }
        private async void ListView_ItemSelectedAsync(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as SlideMenuFlyoutMenuItem;
            if (item == null)
                return;

            if(item.Id == 3)
            {
                
                bool flag = await DisplayAlert("Log out?", "Do you want to log out?", "Yes", "No");
                if (flag)
                {
                    App.logOut();
                } 
                FlyoutPage.ListView.SelectedItem = null;
                return;
            }

            var page = (Page)Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;

            Detail = new NavigationPage(page);
            IsPresented = false;

            FlyoutPage.ListView.SelectedItem = null;
        }
    }
}