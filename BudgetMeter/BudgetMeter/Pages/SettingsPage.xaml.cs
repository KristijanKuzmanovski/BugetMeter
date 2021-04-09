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
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            var theme = Application.Current.UserAppTheme;

            if (string.IsNullOrWhiteSpace(App.firebase.currentUserDetails.pin))
            {
                pinLabel.Text = "Create your pin";
            }
            else
            {
                pinLabel.Text = "Change your pin";
            }

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (string.IsNullOrWhiteSpace(App.firebase.currentUserDetails.pin))
            {
                pinLabel.Text = "Create your pin";
            }
            else
            {
                pinLabel.Text = "Change your pin";
            }
        }

        private void CategoriesLabelTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Pages.CategoryPage(true));
        }
        
        private void SalaryLabelTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ModalPages.IncomeInfoModalPage(false));
        }
        private void UserAccountLabelTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Pages.UserDetailsPage());
        }
       private async void PinLabelTapped(object sender, EventArgs e)
        {

          
            if (string.IsNullOrWhiteSpace(App.firebase.currentUserDetails.pin))
            {
                await Navigation.PushModalAsync(new ModalPages.PinModalPage(true));
            }
            else
            {     
                await Navigation.PushModalAsync(new ModalPages.PinModalPage(false, false, true));
            }

        }

       
    }
}