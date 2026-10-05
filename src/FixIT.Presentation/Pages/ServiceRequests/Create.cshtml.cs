using FixIT.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FixIT.Presentation.Pages.ServiceRequests
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
            var client = _httpClientFactory.CreateClient("FixITApi");
            var response = await client.PostAsJsonAsync("api/servicerequests", Dto);
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
