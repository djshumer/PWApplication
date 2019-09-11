using PWApplication.MobileShared.ViewModels;
using PWApplication.MobileShared.ViewModels.Base;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PWApplication.MobileShared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfileView : ContentPage
    {
        public ProfileView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            (BindingContext as BaseViewModel).InitializeAsync(null);
        }
    }
}