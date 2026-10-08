using FixIT.Application.DTOs;
using FixIT.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FixIT.Presentation.Pages.Technician
{
    [Authorize(Roles = "Tekniker")]
    public class OpenRequestsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public OpenRequestsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<ReadServiceRequestDto> Requests { get; set; } = new(); // lediga ärenden
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("FixITApi");
            var all = await client.GetFromJsonAsync<List<ReadServiceRequestDto>>("api/servicerequests") ?? new();
            Requests = all.Where(r => r.Status == RequestStatus.open).ToList(); // bara öppna
        }

        public async Task<IActionResult> OnPostClaimAsync(int id) // tekniker tar ärendet
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // inloggad tekniker
            var client = _httpClientFactory.CreateClient("FixITApi");
            var response = await client.PutAsync("api/servicerequests/" + id + "/claim?technicianId=" + userId, null);

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = await response.Content.ReadAsStringAsync();
                await OnGetAsync();
                return Page();
            }
            return RedirectToPage("/Technician/MyTasks");
        }
    }
}
