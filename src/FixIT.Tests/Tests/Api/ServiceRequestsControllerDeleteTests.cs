using FixIT.Api.Controllers;
using FixIT.Application.Services;
using FixIT.Domain.Models;
using FixIT.Tests.Fakes;
using Microsoft.AspNetCore.Mvc;

namespace FixIT.Tests.Tests.Api
{
    public class ServiceRequestsControllerDeleteTests
    {
        private readonly FakeServiceRequestRepository _repository = new();
        private readonly ServiceRequestsController _sut;

        public ServiceRequestsControllerDeleteTests()
        {
            _sut = new ServiceRequestsController(new ServiceRequestService(_repository));
        }

        private async Task<int> SeedAsync(string title)
        {
            var request = new ServiceRequests
            {
                Title = title,
                Description = "Beskrivning för " + title,
                Adress = "Testgatan 1",
                ClientId = "1"
            };
            await _repository.AddAsync(request);
            return request.Id;
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContentAndRemoveRequest_WhenRequestExists()
        {
            // Arrange
            var id = await SeedAsync("Läckande tak");

            // Act
            var result = await _sut.Delete(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Empty(_repository.Saved); // ser att requesten har tagits bort alltså att FakeRepository är tom
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenRequestDoesNotExist()
        {
            // Arrange
            await SeedAsync("Läckande tak");

            // Act
            var result = await _sut.Delete(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            Assert.Single(_repository.Saved); // inget har tagits bort. FakeRepository har fortfarande "Läckande tak"
        }
    }
}
