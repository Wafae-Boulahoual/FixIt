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

        [HttpPut("{id}/claim")] //update
        public async Task<IActionResult> ClaimRequest(int id, [FromQuery] string technicianId)
        {
            try
            {
                await _service.ClaimRequestAsync(id, technicianId); // tekniker tar ett ärende
                return NoContent(); //inget att visa
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); // finns inte
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // redan taget
            }

        }


        [HttpPut("{id}/complete")] //update
        public async Task<IActionResult> CompleteRequest(int id, [FromQuery] string technicianId)
        {
            try
            {
                await _service.CompleteRequestAsync(id, technicianId); // tekniker avslutar ett ärende
                return NoContent(); //inget att visa
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); // finns inte
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // redan avslutat
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message); // fel tekniker
            }
        }

        [HttpDelete("{id}")] //delete
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteRequestAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); 
            }
        }


        [HttpPut("{id}")] // update
        public async Task<IActionResult> UpdateRequest(int id, UpdateServiceRequestDto dto, [FromQuery] string clientId)
        {
            try
            {
                await _service.UpdateRequestAsync(id, dto, clientId); // kund ändrar ärendet
                return NoContent(); // inget att visa
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); // finns inte
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // inte öppet
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // tomt fält
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message); // fel kund
            }
        }
    }
}
