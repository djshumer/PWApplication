using System.Threading.Tasks;
using PWApplication.MobileShared.Models.Token;

namespace PWApplication.MobileShared.Services.Identity
{
    public interface IIdentityService
    {
        string CreateAuthorizationRequest();
        string CreateLogoutRequest(string token);
        Task<UserToken> GetTokenAsync(string code);
    }
}
