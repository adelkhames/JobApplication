using Microsoft.AspNetCore.Identity;

namespace JobApplication.Infrastructure.Identity
{
 
    public class AppUser : IdentityUser
    {
        public string Role { get; set; } = string.Empty;


        public string Name { get; set; } = string.Empty;
    }
}
