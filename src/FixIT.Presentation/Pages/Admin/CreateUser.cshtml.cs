using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FixIT.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FixIT.Presentation.Pages.Admin
{
    //[Authorize(Roles = "Admin")]
    public class CreateUserModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public CreateUserModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? SuccessMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Namn måste fyllas i")]
            public string Name { get; set; } = "";

            [Required(ErrorMessage = "E-post måste fyllas i")]
            [EmailAddress]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Lösenord måste fyllas i")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string role)
        {
            // Säkerhet: admin kan bara skapa Kund eller Tekniker härifrån
            if (role != "Kund" && role != "Tekniker")
            {
                ModelState.AddModelError("", "Ogiltig roll.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = new AppUser
            {
                UserName = Input.Email, 
                Email = Input.Email, 
                Name = Input.Name,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return Page();
            }

            await _userManager.AddToRoleAsync(user, role);

            SuccessMessage = $"{role} '{Input.Name}' skapades.";
            ModelState.Clear();
            Input = new InputModel();
            return Page();
        }
    }
}