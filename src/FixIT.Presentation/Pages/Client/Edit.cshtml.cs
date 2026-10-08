using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using FixIT.Application.DTOs;
using FixIT.Domain.Models;

namespace FixIT.Presentation.Pages.Client
{
    [Authorize(Roles = "Kund")]
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [BindProperty]
        public UpdateServiceRequestDto Dto { get; set; } = new();
        public int Id { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id) // hämtar ärendet och fyller formuläret
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // inloggad kund
            var client = _httpClientFactory.CreateClient("FixITApi");
            var response = await client.GetAsync("api/servicerequests/" + id);

            if (response.IsSuccessStatusCode == false)
            {
                return NotFound(); // ärendet finns inte
            }

            var request = await response.Content.ReadFromJsonAsync<ReadServiceRequestDto>();

            if (request == null || request.ClientId != userId || request.Status != RequestStatus.open)
            {
                return NotFound(); // bara egna och öppna ärenden
            }

            Id = id;
            Dto.Title = request.Title;
            Dto.Description = request.Description;
            Dto.Adress = request.Adress;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id) // sparar ändringarna
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var client = _httpClientFactory.CreateClient("FixITApi");
            var response = await client.PutAsJsonAsync("api/servicerequests/" + id + "?clientId=" + userId, Dto);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Client/MyRequests");
            }

            ErrorMessage = await response.Content.ReadAsStringAsync();
            Id = id;
            return Page();
        }
    }
}
