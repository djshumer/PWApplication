using Newtonsoft.Json;

namespace PWApplication.MobileShared.Models.User
{
    public class AppUserInfo
    {
        [JsonProperty("sub")]
        public string UserId { get; set; }

        [JsonProperty("preferred_username")]
        public string PreferredUsername { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("email_verified")]
        public bool EmailVerified { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("phone_number_verified")]
        public bool PhoneNumberVerified { get; set; }
    }
}
