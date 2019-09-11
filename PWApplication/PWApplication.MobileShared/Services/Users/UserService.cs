using System.Threading.Tasks;
using PWApplication.MobileShared.Helpers;
using PWApplication.MobileShared.Models.User;
using PWApplication.MobileShared.Services.RequestProvider;

namespace PWApplication.MobileShared.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IRequestProvider _requestProvider;

        public UserService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        public async Task<AppUserInfo> GetUserInfoAsync(string authToken)
        {
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.UserInfoEndpoint);

            var userInfo = await _requestProvider.GetAsync<AppUserInfo>(uri, authToken);
            return userInfo;
        }
    }
}
