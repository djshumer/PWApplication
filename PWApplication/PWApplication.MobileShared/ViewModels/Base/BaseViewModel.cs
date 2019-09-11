using System.Threading.Tasks;
using System.Windows.Input;
using PWApplication.MobileShared.Models.User;
using PWApplication.MobileShared.Services.DialogService;
using PWApplication.MobileShared.Services.NavigationService;
using Xamarin.Forms;

namespace PWApplication.MobileShared.ViewModels.Base
{
    public abstract class BaseViewModel : ExtendedBindableObject
    {
        protected IDialogService DialogService;
        protected INavigationService NavigationService;

        protected BaseViewModel()
        {
            DialogService = ViewModelLocator.Resolve<IDialogService>();
            NavigationService = ViewModelLocator.Resolve<INavigationService>();
        }

        bool _isReadOnly = false;
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { _isReadOnly = value; RaisePropertyChanged(() => IsReadOnly); }
        }

        bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; RaisePropertyChanged(() => IsBusy); RaisePropertyChanged(() => IsReady); }
        }

        public bool IsReady { get { return !IsBusy; } }

        string _title = string.Empty;
        public string Title
        {
            get { return _title; }
            set { _title = value; RaisePropertyChanged(() => Title); }
        }

        public ICommand LogoutCommand => new Command(async () => await LogoutAsync());

        public virtual Task InitializeAsync(object navigationData)
        {
            return Task.FromResult(false);
        }

        protected async Task LogoutAsync()
        {
            IsBusy = true;

            // Logout
            await NavigationService.NavigateToAsync<LoginViewModel>(new LogoutParameter { Logout = true });
            await NavigationService.RemoveBackStackAsync();

            IsBusy = false;
        }
    }
}
