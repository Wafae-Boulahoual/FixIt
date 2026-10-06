using FixIT.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FixIT.Presentation.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class AllRequestsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AllRequestsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<ReadServiceRequestDto> Requests { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("FixITApi");
            try
            {
                var response = await client.GetAsync("api/servicerequests");
                if (response.IsSuccessStatusCode)
                {
                    var requests = await response.Content.ReadFromJsonAsync<List<ReadServiceRequestDto>>();
                    Requests = (requests ?? new()).OrderByDescending(r => r.CreatedAt).ToList();
                }
                else
                {
                    ErrorMessage = "Kunde inte hämta ärenden från servern.";
                }
            }
            catch (HttpRequestException)
            {
                ErrorMessage = "Kunde inte nå servern. Kontrollera att API:t körs.";
            }
        }
    }
}