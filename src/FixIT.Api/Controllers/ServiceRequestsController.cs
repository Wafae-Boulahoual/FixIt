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
        public async Task<IActionResult> Create(CreateServiceRequestDto dto, [FromQuery] string clientId)
        {
            try // för att kunden kan se meddelandet om en prop är tom
            {
                var id = await _service.CreateServiceRequestAsync(dto, clientId);
                return Created("api/servicerequests/" + id, new { id });
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _service.GetAllAsync();
            return Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var request = await _service.GetByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            return Ok(request);
        }

        [HttpGet("client/{clientId}")]
        public async Task<IActionResult> GetClientHistory(string clientId)
        {
            var requests = await _service.GetClientHistoryAsync(clientId);
            return Ok(requests);
        }
    }
}
