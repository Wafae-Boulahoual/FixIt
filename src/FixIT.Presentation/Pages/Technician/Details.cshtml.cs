using FixIT.Application.DTOs;
using FixIT.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FixIT.Presentation.Pages.Technician
{
    [Authorize(Roles = "Tekniker")]
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<AppUser> _userManager;

        public DetailsModel(IHttpClientFactory httpClientFactory, UserManager<AppUser> userManager)
        {
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
        }

        public ReadServiceRequestDto? ServiceRequest { get; set; }
        public string? ClientName { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("FixITApi");
            var response = await client.GetAsync("api/servicerequests/" + id);

            if (response.IsSuccessStatusCode == false)
            {
                return NotFound(); // ärendet finns inte
            }

            ServiceRequest = await response.Content.ReadFromJsonAsync<ReadServiceRequestDto>();

            if (ServiceRequest == null)
            {
                return NotFound(); // ärendet finns inte
            }

            var kund = await _userManager.FindByIdAsync(ServiceRequest.ClientId); // hämtar kunden
            if (kund != null)
            {
                ClientName = kund.Name; // kundens namn
            }

            return Page();
        }
    }
}