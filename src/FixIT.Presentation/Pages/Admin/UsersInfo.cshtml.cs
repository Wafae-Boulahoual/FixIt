using FixIT.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FixIT.Presentation.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersInfoModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public UsersInfoModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public List<AppUser> Customers { get; set; } = new();   
        public List<AppUser> Technicians { get; set; } = new(); 

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData] // används för att skicka meddelanden mellan requests
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            Customers = (await _userManager.GetUsersInRoleAsync("Kund"))
                .OrderBy(u => u.Name).ToList();
            Technicians = (await _userManager.GetUsersInRoleAsync("Tekniker"))
                .OrderBy(u => u.Name).ToList();
        }

        // körs när admin har bekräftat i dialogen att kontot ska tas bort
        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
            {
                ErrorMessage = "Kontot finns inte.";
                return RedirectToPage(); // skickar tillbaka till samma sida
            }

            if (user.Id == _userManager.GetUserId(User))
            {
                ErrorMessage = "Du kan inte ta bort ditt eget konto.";
                return RedirectToPage();
            }

            var isCustomer = await _userManager.IsInRoleAsync(user, "Kund");
            var isTechnician = await _userManager.IsInRoleAsync(user, "Tekniker");
            if (!isCustomer && !isTechnician)
            {
                ErrorMessage = "Bara kunder och tekniker kan tas bort här.";
                return RedirectToPage(); 
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                SuccessMessage = $"Kontot '{user.Name ?? user.Email}' har tagits bort.";
            }
            else
            {
                ErrorMessage = "Kunde inte ta bort kontot.";
            }
            return RedirectToPage();
        }
    }
}