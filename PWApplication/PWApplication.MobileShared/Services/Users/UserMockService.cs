using System;
using System.Threading.Tasks;
using PWApplication.MobileShared.Models.User;

namespace PWApplication.MobileShared.Services.Users
{
    public class UserMockService : IUserService
    {
        private AppUserInfo MockUserInfo = new AppUserInfo
        {
            UserId = Guid.NewGuid().ToString(),
            FullName = "Jokhn Wick",
            Email = "jdoe@eshop.com",
            EmailVerified = false,
            PhoneNumber = "202-555-0165",
            PhoneNumberVerified = false,
        };

        public async Task<AppUserInfo> GetUserInfoAsync(string authToken)
        {
            await Task.Delay(10);
            return MockUserInfo;
        }
    }
}
