using PWApplication.MobileShared.CustomControls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PWApplication.MobileShared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginView : ContentPage
    {
        public LoginView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            AuthorWebView.ShouldTrustUnknownCertificate = ShouldTrustUnknownCertificate;
        }

        private bool ShouldTrustUnknownCertificate(ICustomCertificate certificate)
        {
            return (certificate.Host.Contains("localhost") || certificate.Host.Contains("10.0.2.2"));
        }

    }
}