using System.Net;
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

        // låtsas vara API:t, svarar med vald statuskod och sparar anropet
        private class StubHandler(HttpStatusCode status, string body = "") : HttpMessageHandler
        {
            public HttpRequestMessage? LastRequest { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                LastRequest = request;
                return Task.FromResult(new HttpResponseMessage(status) { Content = new StringContent(body) });
            }
        }

        private class ThrowingHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => throw new HttpRequestException("API:t körs inte");
        }

        private static UsersInfoModel CreateSut(HttpMessageHandler handler)
        {
            var store = new Mock<IUserStore<AppUser>>();
            var mockUserManager = new Mock<UserManager<AppUser>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            mockUserManager.Setup(m => m.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(AdminId);

            var factory = new Mock<IHttpClientFactory>();
            factory.Setup(f => f.CreateClient("FixITApi"))
                   .Returns(new HttpClient(handler) { BaseAddress = new Uri("https://localhost/") });

            return new UsersInfoModel(mockUserManager.Object, factory.Object)
            {
                PageContext = new PageContext { HttpContext = new DefaultHttpContext() }
            };
        }

        [Fact]
        public async Task OnPostDeleteAsyncShouldCallApiAndShowSuccess()
        {
            // Arrange
            var handler = new StubHandler(HttpStatusCode.NoContent);
            var sut = CreateSut(handler);

            // Act
            var result = await sut.OnPostDeleteAsync("user-1");

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(HttpMethod.Delete, handler.LastRequest!.Method);
            Assert.Equal("/api/users/user-1", handler.LastRequest.RequestUri!.AbsolutePath);
            Assert.Contains("currentAdminId=admin-1", handler.LastRequest.RequestUri.Query);
            Assert.Equal("Kontot har tagits bort.", sut.SuccessMessage);
            Assert.Null(sut.ErrorMessage);
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest, "Du kan inte ta bort ditt eget konto.")]
        [InlineData(HttpStatusCode.BadRequest, "Bara kunder och tekniker kan tas bort här.")]
        [InlineData(HttpStatusCode.NotFound, "Kontot finns inte.")]
        public async Task OnPostDeleteAsyncShouldShowErrorFromApi(HttpStatusCode status, string message)
        {
            // Arrange
            var sut = CreateSut(new StubHandler(status, message));

            // Act
            var result = await sut.OnPostDeleteAsync("user-1");

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(message, sut.ErrorMessage);
            Assert.Null(sut.SuccessMessage);
        }

        [Fact]
        public async Task OnPostDeleteAsyncShouldShowErrorWhenApiIsDown()
        {
            // Arrange
            var sut = CreateSut(new ThrowingHandler());

            // Act
            var result = await sut.OnPostDeleteAsync("user-1");

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Kunde inte nå servern. Kontrollera att API:t körs.", sut.ErrorMessage);
            Assert.Null(sut.SuccessMessage);
        }
    }
}