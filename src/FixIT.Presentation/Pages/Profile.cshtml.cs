using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FixIT.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FixIT.Presentation.Pages
{
    [Authorize] // alla inloggade
    public class ProfileModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        public ProfileModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public async Task<IActionResult>OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                Name = user.Name;
                Email = user.Email;
                PhoneNumber = user.PhoneNumber;
            }
            return Page();
        }
    }
}
