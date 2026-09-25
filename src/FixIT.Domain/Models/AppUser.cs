using Microsoft.AspNetCore.Identity;

namespace FixIT.Domain.Models
{
    public class AppUser : IdentityUser
    {
        public string? Name { get; set; } = "";
    }
}