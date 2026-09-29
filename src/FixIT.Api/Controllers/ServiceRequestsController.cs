using FixIT.Application.DTOs;
using FixIT.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FixIT.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestService _service;
        public ServiceRequestsController(IServiceRequestService service)
        {
            _service = service;
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateServiceRequestDto dto)
        {
            int clienId = 1; //temporär
            var id = await _service.CreateServiceRequestAsync(dto, clienId);
            return Created("api/servicerequests/" + id, new { id });
        }
    }
}
