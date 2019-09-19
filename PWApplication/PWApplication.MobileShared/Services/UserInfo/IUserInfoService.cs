using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PWApplication.MobileShared.Models.User;

namespace PWApplication.MobileShared.Services.UserInfo
{
    public interface IUserInfoService
    {
        Task<ObservableCollection<UserInfoSimple>> FindUserAsync(string authToken, string username);

        Task<UserInfoSimple> GetUserInfoAsync(string authToken, string userId);

        Task<AppUserInfo> GetCurrentUserInfoAsync(string authToken);
    }
}