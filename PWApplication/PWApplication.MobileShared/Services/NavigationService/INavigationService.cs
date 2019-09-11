using System.Threading.Tasks;
using PWApplication.MobileShared.ViewModels;
using PWApplication.MobileShared.ViewModels.Base;

namespace PWApplication.MobileShared.Services.NavigationService
{
    public interface INavigationService
    {
        BaseViewModel PreviousPageViewModel { get; }

        Task InitializeAsync();

        Task NavigateToAsync<TViewModel>() where TViewModel : BaseViewModel;

        Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : BaseViewModel;

        Task NavigationToBackAsync();

        Task RemoveLastFromBackStackAsync();

        Task RemoveBackStackAsync();
    }
}
