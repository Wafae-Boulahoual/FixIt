using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using FixIT.Application.DTOs;

namespace FixIT.Presentation.Pages.Client
{
    [Authorize(Roles = "Kund")]
    public class MyRequestsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public MyRequestsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public List<ReadServiceRequestDto> Requests { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //inloggad kund Id
            var client = _httpClientFactory.CreateClient("FixITApi");

            try
            {
                Requests = await client.GetFromJsonAsync<List<ReadServiceRequestDto>>("api/serviceRequests/client/" + userId) ?? new ();
            }
            catch (HttpRequestException)
            {
                ErrorMessage = "Ett fel uppstod vid hämtning av felanmälningar.";
            }
        }
    }
}
