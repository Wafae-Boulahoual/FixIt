using FixIT.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FixIT.Presentation.Pages.Client
{
    [Authorize(Roles = "Kund")]
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [BindProperty]
        public CreateServiceRequestDto Dto { get; set; } = new ();
        public string? ErrorMessage { get; set; }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //hämtar inloggad användarens Id
            var client = _httpClientFactory.CreateClient("FixITApi");
            var response = await client.PostAsJsonAsync("api/servicerequests?clientId=" + userId, Dto);
            if(response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessage = await response.Content.ReadAsStringAsync();
            }
            return Page();
        }
    }
}
