using Microsoft.AspNetCore.Identity;

namespace Transaction.Api.Infrastructure.Data.DataModels
{
    public class ApplicationUser : IdentityUser
    {       
        public string FullName { get; set; }
    }
}
