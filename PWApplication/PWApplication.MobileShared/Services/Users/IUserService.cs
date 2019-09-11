using System.Threading.Tasks;
using PWApplication.MobileShared.Models.User;

namespace PWApplication.MobileShared.Services.Users
{
    public interface IUserService
    {
        Task<AppUserInfo> GetUserInfoAsync(string authToken);
    }
}
