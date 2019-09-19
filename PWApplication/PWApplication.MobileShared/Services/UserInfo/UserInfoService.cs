using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PWApplication.MobileShared.Extensions;
using PWApplication.MobileShared.Helpers;
using PWApplication.MobileShared.Models.User;
using PWApplication.MobileShared.Services.RequestProvider;

namespace PWApplication.MobileShared.Services.UserInfo
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IRequestProvider _requestProvider;

        private const string ApiUrlBase = "api/v1/userinfo";

        public UserInfoService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        // GET api/v1/userinfo/find[?username="dem"]
        public async Task<ObservableCollection<UserInfoSimple>> FindUserAsync(string authToken, string username)
        {
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewayPWEndpoint, $"{ApiUrlBase}/find?username={username}");

            var transactionsList = await _requestProvider.GetAsync<IEnumerable<UserInfoSimple>>(uri, authToken);

            if (transactionsList != null)
                return transactionsList.ToObservableCollection();
            else
                return new ObservableCollection<UserInfoSimple>();
        }

        // GET api/v1/userinfo/{userId}]
        public async Task<UserInfoSimple> GetUserInfoAsync(string authToken, string userId)
        {
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewayPWEndpoint, $"{ApiUrlBase}/{userId}");

            var userInfo = await _requestProvider.GetAsync<UserInfoSimple>(uri, authToken);

            return userInfo;
        }

        public async Task<AppUserInfo> GetCurrentUserInfoAsync(string authToken)
        {
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.UserInfoEndpoint);

            var userInfo = await _requestProvider.GetAsync<AppUserInfo>(uri, authToken);

            return userInfo;
        }
    }
}
