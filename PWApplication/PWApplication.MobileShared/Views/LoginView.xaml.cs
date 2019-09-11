using PWApplication.MobileShared.CustomControls;
using PWApplication.MobileShared.Services.DialogService;
using PWApplication.MobileShared.ViewModels;
using PWApplication.MobileShared.ViewModels.Base;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PWApplication.MobileShared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginView : ContentPage
    {
        private bool _animate;

        public LoginView()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            AuthorWebView.ShouldTrustUnknownCertificate = ShouldTrustUnknownCertificate;
        }

        private bool ShouldTrustUnknownCertificate(ICustomCertificate certificate)
        {
            return (certificate.Host.Contains("localhost") || certificate.Host.Contains("10.0.2.2"));
        }

    }
}