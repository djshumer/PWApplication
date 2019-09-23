

using Microsoft.AspNetCore.Identity;

namespace PWApplication.Identity.Models.DataModels
{
    public class ApplicationUser : IdentityUser
    {
       
        public string FullName { get; set; }
    }
}
