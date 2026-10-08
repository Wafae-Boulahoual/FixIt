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
            _mockUserManager = new Mock<UserManager<AppUser>>(
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
            _mockUserManager.Verify(m => m.DeleteAsync(user), Times.Once); // tas bort en gång
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Contains("Anna Andersson", _sut.SuccessMessage);
            Assert.Null(_sut.ErrorMessage);
        }


    }
}