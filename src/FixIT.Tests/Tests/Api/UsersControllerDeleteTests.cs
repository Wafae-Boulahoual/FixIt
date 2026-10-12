using FixIT.Api.Controllers;
using FixIT.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FixIT.Tests.Tests.Api
{
    public class UsersControllerDeleteTests
    {
        private const string AdminId = "admin-1"; 

        private readonly Mock<UserManager<AppUser>> _mockUserManager;
        private readonly UsersController _sut;

        public UsersControllerDeleteTests()
        {
            // UserManager kräver 9 parametrar, men vi mockar bara store
            var store = new Mock<IUserStore<AppUser>>();
            _mockUserManager = new Mock<UserManager<AppUser>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!); 

            _sut = new UsersController(_mockUserManager.Object);
        }

        [Theory]
        [InlineData("Kund")]
        [InlineData("Tekniker")]
        public async Task Delete_ShouldReturnNoContent_WhenUserIsCustomerOrTechnician(string role)
        {
            // Arrange
            var user = new AppUser { Id = "user-1", Name = "Anna Andersson", Email = "anna@test.se" };
            _mockUserManager.Setup(m => m.FindByIdAsync("user-1")).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.IsInRoleAsync(user, role)).ReturnsAsync(true);
            _mockUserManager.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _sut.Delete("user-1", AdminId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockUserManager.Verify(m => m.DeleteAsync(user), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturnBadRequest_WhenAdminDeletesOwnAccount()
        {
            // Arrange
            var admin = new AppUser { Id = AdminId, Name = "Admin", Email = "admin@fixit.se" };
            _mockUserManager.Setup(m => m.FindByIdAsync(AdminId)).ReturnsAsync(admin);
            _mockUserManager.Setup(m => m.IsInRoleAsync(admin, "Admin")).ReturnsAsync(true);

            // Act
            var result = await _sut.Delete(AdminId, AdminId);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Du kan inte ta bort ditt eget konto.", badRequest.Value);
            _mockUserManager.Verify(m => m.DeleteAsync(It.IsAny<AppUser>()), Times.Never);
        }

        [Fact]
        public async Task Delete_ShouldReturnBadRequest_WhenUserIsAnotherAdmin()
        {
            // Arrange
            var otherAdmin = new AppUser { Id = "admin-2", Name = "Annan Admin", Email = "admin2@fixit.se" };
            _mockUserManager.Setup(m => m.FindByIdAsync("admin-2")).ReturnsAsync(otherAdmin);
            _mockUserManager.Setup(m => m.IsInRoleAsync(otherAdmin, "Admin")).ReturnsAsync(true);
            // "Kund" och "Tekniker" är inte uppsatta och ger därför false

            // Act
            var result = await _sut.Delete("admin-2", AdminId);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bara kunder och tekniker kan tas bort här.", badRequest.Value);
            _mockUserManager.Verify(m => m.DeleteAsync(It.IsAny<AppUser>()), Times.Never);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            _mockUserManager.Setup(m => m.FindByIdAsync("finns-inte")).ReturnsAsync((AppUser?)null);

            // Act
            var result = await _sut.Delete("finns-inte", AdminId);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Kontot finns inte.", notFound.Value);
            _mockUserManager.Verify(m => m.DeleteAsync(It.IsAny<AppUser>()), Times.Never);
        }
    }
}