using System.Security.Claims;
using FixIT.Domain.Models;
using FixIT.Presentation.Pages.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace FixIT.Tests.Tests
{
    public class DeleteUsersTests
    {
        private const string AdminId = "admin-1";

        private readonly Mock<UserManager<AppUser>> _mockUserManager;
        private readonly UsersInfoModel _sut;

        public DeleteUsersTests()
        {
            // UserManager har ingen parameterlös konstruktor, så mocken behöver konstruktorargumenten (bara store behövs)
            var store = new Mock<IUserStore<AppUser>>(); 
            _mockUserManager = new Mock<UserManager<AppUser>>( //UserManager kräver 9 parametrar, men vi mockar bara store och andra kan vara null
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!); 

            // admin är inloggad som "admin-1"
            _mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(AdminId);

            _sut = new UsersInfoModel(_mockUserManager.Object)
            {
                PageContext = new PageContext { HttpContext = new DefaultHttpContext() }
            };
        }

        [Theory] // en kund eller tekniker kan tas bort
        [InlineData("Kund")]
        [InlineData("Tekniker")]
        public async Task OnPostDeleteAsyncShouldDeleteCustomerOrTechnician(string role)
        {
            // Arrange
            var user = new AppUser { Id = "user-1", Name = "Anna Andersson", Email = "anna@test.se" };
            _mockUserManager.Setup(m => m.FindByIdAsync("user-1")).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.IsInRoleAsync(user, role)).ReturnsAsync(true);
            _mockUserManager.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _sut.OnPostDeleteAsync("user-1");

            // Assert
            _mockUserManager.Verify(m => m.DeleteAsync(user), Times.Once); 
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Contains("Anna Andersson", _sut.SuccessMessage);
            Assert.Null(_sut.ErrorMessage);
        }


        [Fact] 
        public async Task OnPostDeleteAsyncShouldNotDeleteOwnAccount()
        {
            // Arrange
            var admin = new AppUser { Id = AdminId, Name = "Admin", Email = "admin@fixit.se" };
            _mockUserManager.Setup(m => m.FindByIdAsync(AdminId)).ReturnsAsync(admin);
            _mockUserManager.Setup(m => m.IsInRoleAsync(admin, "Admin")).ReturnsAsync(true);

            // Act
            var result = await _sut.OnPostDeleteAsync(AdminId);

            // Assert
            _mockUserManager.Verify(m => m.DeleteAsync(It.IsAny<AppUser>()), Times.Never); // tas aldrig bort
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Du kan inte ta bort ditt eget konto.", _sut.ErrorMessage);
            Assert.Null(_sut.SuccessMessage);
        }

        [Fact] 
        public async Task OnPostDeleteAsyncShouldNotDeleteAnotherAdmin()
        {
            // Arrange
            var otherAdmin = new AppUser { Id = "admin-2", Name = "Annan Admin", Email = "admin2@fixit.se" };
            _mockUserManager.Setup(m => m.FindByIdAsync("admin-2")).ReturnsAsync(otherAdmin);
            _mockUserManager.Setup(m => m.IsInRoleAsync(otherAdmin, "Admin")).ReturnsAsync(true);
            // IsInRoleAsync för "Kund" och "Tekniker" är inte uppsatt och ger därför false

            // Act
            var result = await _sut.OnPostDeleteAsync("admin-2");

            // Assert
            _mockUserManager.Verify(m => m.DeleteAsync(It.IsAny<AppUser>()), Times.Never); // tas aldrig bort
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Bara kunder och tekniker kan tas bort här.", _sut.ErrorMessage);
            Assert.Null(_sut.SuccessMessage);
        }

        [Fact] // kontot finns inte
        public async Task OnPostDeleteAsyncShouldShowErrorIfUserDoesNotExist()
        {
            // Arrange
            _mockUserManager.Setup(m => m.FindByIdAsync("finns-inte")).ReturnsAsync((AppUser?)null);

            // Act
            var result = await _sut.OnPostDeleteAsync("finns-inte");

            // Assert
            _mockUserManager.Verify(m => m.DeleteAsync(It.IsAny<AppUser>()), Times.Never); // tas aldrig bort
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Kontot finns inte.", _sut.ErrorMessage);
            Assert.Null(_sut.SuccessMessage);
        }
    }
}