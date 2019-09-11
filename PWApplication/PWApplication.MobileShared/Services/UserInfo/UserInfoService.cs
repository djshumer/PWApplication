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
        public async Task<ObservableCollection<UserInfoSimple>> FindUser(string authToken, string username)
        {
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewayPWEndpoint, $"{ApiUrlBase}/find?username={username}");

            var transactionsList = await _requestProvider.GetAsync<IEnumerable<UserInfoSimple>>(uri, authToken);

            if (transactionsList != null)
                return transactionsList.ToObservableCollection();
            else
                return new ObservableCollection<UserInfoSimple>();
        }

        // GET api/v1/userinfo/{userId}]
        public async Task<UserInfoSimple> GetUserInfo(string authToken, string userId)
        {
            var uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewayPWEndpoint, $"{ApiUrlBase}/{userId}");

            var userInfo = await _requestProvider.GetAsync<UserInfoSimple>(uri, authToken);

            return userInfo;
        }
    }
}
