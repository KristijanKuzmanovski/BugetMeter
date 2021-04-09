using ImageCircle.Forms.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BudgetMeter.SlideMenu
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SlideMenuFlyout : ContentPage
    {
        public ListView ListView;

        public SlideMenuFlyout()
        {
            InitializeComponent();
            BindingContext = new SlideMenuFlyoutViewModel();
            
            ListView = MenuItemsListView;
            checkUserDetails();
        }
        public void checkUserDetails()
        {
            string _avatar = App.firebase.currentUser.PhotoUrl;
            if (!string.IsNullOrWhiteSpace(_avatar))
            {
                avatar.Source = ImageSource.FromUri(new Uri(_avatar));
            }
            else
            {
                avatar.Source = ImageSource.FromResource("BudgetMeter.Images.profile.png");
            }
            Name_Label.Text = App.firebase.currentUser.DisplayName;
            
        }

        class SlideMenuFlyoutViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<SlideMenuFlyoutMenuItem> MenuItems { get; set; }

            public SlideMenuFlyoutViewModel()
            {
                MenuItems = new ObservableCollection<SlideMenuFlyoutMenuItem>(new[]
                {
                    new SlideMenuFlyoutMenuItem { Id = 0, Title = "Home", TargetType = typeof(MainPage), Icon = "home.png" },
                    new SlideMenuFlyoutMenuItem { Id = 1, Title = "Reports", TargetType = typeof(Pages.ReportsPage), Icon = "documents.png" },
                    new SlideMenuFlyoutMenuItem { Id = 2, Title = "Settings", TargetType = typeof(Pages.SettingsPage), Icon = "settings.png" },
                    new SlideMenuFlyoutMenuItem { Id = 3, Title = "Logout", Icon = "logout.png" },
                    //new SlideMenuFlyoutMenuItem { Id = 3, Title = "Page 4" },
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Pages.UserDetailsPage());
        }
    }
}