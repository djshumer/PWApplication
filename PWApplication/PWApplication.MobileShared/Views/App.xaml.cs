using System.Threading.Tasks;
using PWApplication.MobileShared.Services.NavigationService;
using PWApplication.MobileShared.Services.Settings;
using PWApplication.MobileShared.ViewModels.Base;
using Xamarin.Forms;

namespace PWApplication.MobileShared.Views
{
    public partial class App : Application
    {
        ISettingsService _settingsService;

        public App()
        {
            InitializeComponent();

            InitApp();

            if (Device.RuntimePlatform == Device.UWP)
            {
                InitNavigation();
            }
        }

        private Task InitNavigation()
        {
            var navigationService = ViewModelLocator.Resolve<INavigationService>();
            return navigationService.InitializeAsync();
        }

        private void InitApp()
        {
            _settingsService = ViewModelLocator.Resolve<ISettingsService>();
            //TODO:UseMock false for backend test and use Android Emulator with api level 23-26 
            _settingsService.UseMocks = false;
            if (_settingsService.UseMocks)
                ViewModelLocator.UpdateDependencies(_settingsService.UseMocks);
        }

        protected override async void OnStart()
        {
            base.OnStart();

            if (Device.RuntimePlatform != Device.UWP)
            {
                await InitNavigation();
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
