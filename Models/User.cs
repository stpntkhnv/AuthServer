using Microsoft.AspNetCore.Identity;

namespace AuthServer.Models
{
    public class User : IdentityUser
    {
        public string Password { get; set; }
        public UserProfile Profile { get; set; }
    }
}