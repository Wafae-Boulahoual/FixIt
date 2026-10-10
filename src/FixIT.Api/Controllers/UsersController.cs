using FixIT.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FixIT.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        public UsersController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpDelete("{id}")] // delete
        public async Task<IActionResult> Delete(string id, [FromQuery] string currentAdminId)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
            {
                return NotFound("Kontot finns inte.");
            }

            if (user.Id == currentAdminId)
            {
                return BadRequest("Du kan inte ta bort ditt eget konto.");
            }

            var isCustomer = await _userManager.IsInRoleAsync(user, "Kund");
            var isTechnician = await _userManager.IsInRoleAsync(user, "Tekniker");
            if (!isCustomer && !isTechnician)
            {
                return BadRequest("Bara kunder och tekniker kan tas bort här.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest("Kunde inte ta bort kontot.");
            }
            return NoContent();
        }
    }
}