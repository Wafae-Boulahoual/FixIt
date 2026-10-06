using FixIT.Application.DTOs;
using FixIT.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FixIT.Presentation.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class AllRequestsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<AppUser> _userManager;

        public AllRequestsModel(IHttpClientFactory httpClientFactory, UserManager<AppUser> userManager)
        {
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
        }

        public List<ReadServiceRequestDto> Requests { get; set; } = new();
        public Dictionary<string, string> ClientNames { get; set; } = new(); // kund-id => namn
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

                    // hämta namnet för varje kund som har ett ärende
                    foreach (var clientId in Requests.Select(r => r.ClientId).Distinct())
                    {
                        if (string.IsNullOrEmpty(clientId)) continue;

                        var user = await _userManager.FindByIdAsync(clientId);
                        if (user != null)
                        {
                            ClientNames[clientId] = user.Name ?? user.Email ?? "Okänd kund";
                        }
                    }
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