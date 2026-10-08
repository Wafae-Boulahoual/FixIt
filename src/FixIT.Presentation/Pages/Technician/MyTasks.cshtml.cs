using Microsoft.AspNetCore.Authorization;
using FixIT.Application.DTOs;
using FixIT.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FixIT.Presentation.Pages.Technician
{
    [Authorize(Roles = "Tekniker")]
    public class MyTasksModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public MyTasksModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public List<ReadServiceRequestDto> Requests { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // inloggad tekniker
            var client = _httpClientFactory.CreateClient("FixITApi");
            var all = await client.GetFromJsonAsync<List<ReadServiceRequestDto>>("api/servicerequests") ?? new();
            Requests = all.Where(r => r.TechnichianId == userId).OrderBy(r => r.Status).ToList(); //först pågående sen avslutad (som enum)
        }

        public async Task<IActionResult> OnPostCompleteAsync(int id) // tekniker avslutar ärendet
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var client = _httpClientFactory.CreateClient("FixITApi");
            var response = await client.PutAsync("api/servicerequests/" + id + "/complete?technicianId=" + userId, null);

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = await response.Content.ReadAsStringAsync();
                await OnGetAsync();
                return Page();
            }
            return RedirectToPage();
        }
    }
    
}
