using FixIT.Application.DTOs;
using FixIT.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace FixIT.Presentation.Pages
{
    [Authorize] // kräver inloggning
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DetailsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public ReadServiceRequestDto? ServiceRequest { get; set; }
        public string BackPage { get; set; } = "/Index"; // vart "Tillbaka" ska gå

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var client = _httpClientFactory.CreateClient("FixITApi");
            var response = await client.GetAsync("api/servicerequests/" + id);

            if (response.IsSuccessStatusCode == false)
            {
                return NotFound(); // ärendet finns inte
            }

            ServiceRequest = await response.Content.ReadFromJsonAsync<ReadServiceRequestDto>();

            if (ServiceRequest == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Admin"))
            {
                BackPage = "/Admin/AllRequests"; // admin får se alla ärenden
            }
            else if (User.IsInRole("Tekniker"))
            {
                // tekniker får se öppna ärenden och sina egna
                if (ServiceRequest.Status != RequestStatus.open && ServiceRequest.TechnichianId != userId)
                {
                    return NotFound();
                }
                BackPage = ServiceRequest.Status == RequestStatus.open
                    ? "/Technician/OpenRequests"
                    : "/Technician/MyTasks";
            }
            else if (User.IsInRole("Kund"))
            {
                // kunden får bara se sina egna ärenden
                if (ServiceRequest.ClientId != userId)
                {
                    return NotFound();
                }
                BackPage = "/Client/MyRequests";
            }
            else
            {
                return Forbid(); // inloggad men saknar roll
            }

            return Page();
        }
    }
}