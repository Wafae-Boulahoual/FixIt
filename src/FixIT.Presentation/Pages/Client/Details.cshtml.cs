using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FixIT.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;




namespace FixIT.Presentation.Pages.Client
{
    [Authorize(Roles ="Kund")]
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public DetailsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public ReadServiceRequestDto? ServiceRequest { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // inloggad kund
            var client = _httpClientFactory.CreateClient("FixITApi");
            var response = await client.GetAsync("api/servicerequests/" + id);

            if (response.IsSuccessStatusCode == false)
            {
                return NotFound(); // ärendet finns inte
            }

            ServiceRequest = await response.Content.ReadFromJsonAsync<ReadServiceRequestDto>();

            if (ServiceRequest == null || ServiceRequest.ClientId != userId)
            {
                return NotFound(); // kunden får bara se sina egna ärenden
            }

            return Page();
        }
    }
}
