using PWApplication.MobileShared.Models.User;
using PWApplication.MobileShared.Services.Settings;
using PWApplication.MobileShared.Services.UserInfo;
using PWApplication.MobileShared.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using PWApplication.MobileShared.Extensions;
using PWApplication.MobileShared.Services.RequestProvider;
using System.Diagnostics;

namespace PWApplication.MobileShared.ViewModels
{
    public class SearchUserViewModel : BaseViewModel
    {

        private readonly IUserInfoService _userInfoService;
        private readonly ISettingsService _settingsService;

        private string _searchQuery;
        private ObservableCollection<UserInfoSimple> _searchResults = new ObservableCollection<UserInfoSimple>();

        public SearchUserViewModel(IUserInfoService userInfoService, ISettingsService settingsService)
        {
            _userInfoService = userInfoService;
            _settingsService = settingsService;

            Device.StartTimer(TimeSpan.FromSeconds(2), RefreshSearchCallBack);
        }

        private bool _searchTextChanged = false;
        public bool IsSearchQueryChanged
        {
            get { return _searchTextChanged; }
            set { _searchTextChanged = value; RaisePropertyChanged(() => IsSearchQueryChanged); }
        }

        public string SearchQuery
        {
            get { return _searchQuery; }
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value;
                    RaisePropertyChanged(() => SearchQuery);
                    IsSearchQueryChanged = true;
                }
            }
        }

        private bool RefreshSearchCallBack()
        {
            if (IsSearchQueryChanged && IsBusy == false)
            {
                SearchAsync(SearchQuery);
            }
            return true;
        }


        public ICommand PerformSearchCommand => new Command<string>(async (string query) => await SearchAsync(query));

        public ICommand SelectedUserCommand => new Command<UserInfoSimple>(async (UserInfoSimple user) => await SelectedUserHandlerAsync(user));

        private async Task SelectedUserHandlerAsync(UserInfoSimple user)
        {
            if (NavigationService.PreviousPageViewModel is NewTransactionViewModel)
            {
                (NavigationService.PreviousPageViewModel as NewTransactionViewModel).SetCounteragent(user);
                //SearchQuery = user.FullName;
                await NavigationService.NavigationToBackAsync();
            }
        }

        private async Task SearchAsync(string query)
        {
            if (IsBusy)
                return;

            IsSearchQueryChanged = false;

            if (String.IsNullOrWhiteSpace(query) || query.Length < 3)
                return;

            IsBusy = true;

            try
            {
                var authToken = _settingsService.AuthAccessToken;
                var results = await _userInfoService.FindUserAsync(authToken, query);
                SearchResults = results.ToObservableCollection();
            }
            catch (ServiceAuthenticationException)
            {
                await LogoutAsync();
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Error retrieve data: " + exc.Message);
                DialogService.ShowInformationUserMessage(this, "unable to retrieve data, check for Internet availability", "Cancel");
            }
            finally
            {
                IsBusy = false;
            }
        }
                
        public ObservableCollection<UserInfoSimple> SearchResults
        {
            get { return _searchResults; }
            set { _searchResults = value; RaisePropertyChanged(() => SearchResults); }
        }

        private void StartDelay(TimeSpan timeSpan, Func<bool> callBack)
        {
            Device.StartTimer(timeSpan, callBack);
        }

        
    }
}
