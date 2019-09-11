using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PWApplication.MobileShared.Models.User;

namespace PWApplication.MobileShared.Services.UserInfo
{
    public interface IUserInfoService
    {
        Task<ObservableCollection<UserInfoSimple>> FindUser(string authToken, string username);

        Task<UserInfoSimple> GetUserInfo(string authToken, string userId);
    }
}