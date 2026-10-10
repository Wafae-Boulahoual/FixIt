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
        private readonly IHttpClientFactory _httpClientFactory;

        public UsersInfoModel(UserManager<AppUser> userManager, IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _httpClientFactory = httpClientFactory;
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
            var client = _httpClientFactory.CreateClient("FixITApi");
            var currentAdminId = _userManager.GetUserId(User) ?? "";
            try
            {
                var response = await client.DeleteAsync(
                    "api/users/" + Uri.EscapeDataString(id) + "?currentAdminId=" + Uri.EscapeDataString(currentAdminId));
                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Kontot har tagits bort.";
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync(); // felmeddelandet kommer från API:t
                    ErrorMessage = string.IsNullOrWhiteSpace(message) ? "Kunde inte ta bort kontot." : message;
                }
            }
            catch (HttpRequestException)
            {
                ErrorMessage = "Kunde inte nå servern. Kontrollera att API:t körs.";
            }
            return RedirectToPage();
        }
    }
}