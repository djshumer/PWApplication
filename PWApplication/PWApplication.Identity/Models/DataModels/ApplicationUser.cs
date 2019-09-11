

using Microsoft.AspNetCore.Identity;

namespace PWApplication.MobileAppService.Models.DataModels
{
    public class ApplicationUser : IdentityUser
    {
       
        public string FullName { get; set; }
    }
}
