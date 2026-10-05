using FixIT.Api.Controllers;
using FixIT.Application.DTOs;
using FixIT.Application.Services;
using FixIT.Domain.Models;
using FixIT.Tests.Fakes;
using Microsoft.AspNetCore.Mvc;

namespace FixIT.Tests.Tests.Api
{
    public class ServiceRequestsControllerReadTests
    {
        private readonly FakeServiceRequestRepository _repository = new();
        private readonly ServiceRequestsController _sut;

        public ServiceRequestsControllerReadTests()
        {
            _sut = new ServiceRequestsController(new ServiceRequestService(_repository));
        }

        private async Task<int> SeedAsync(string title, int clientId = 1)
        {
            var request = new ServiceRequests
            {
                Title = title,
                Description = "Beskrivning för " + title,
                Adress = "Testgatan 1",
                ClientId = clientId
            };
            await _repository.AddAsync(request);
            return request.Id;
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyList_WhenNoRequestsExist()
        {

            // Act
            var result = await _sut.GetAll();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<ReadServiceRequestDto>>(ok.Value);
            Assert.Empty(list);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllRequests()
        {
            // Arrange
            await SeedAsync("Läckande tak");
            await SeedAsync("Trasig dörr");

            // Act
            var result = await _sut.GetAll();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<ReadServiceRequestDto>>(ok.Value); 
            Assert.Equal(2, list.Count());
        }

        [Fact]
        public async Task GetById_ShouldReturnDto_WhenRequestExists()
        {
            // Arrange
            var id = await SeedAsync("Läckande tak");

            // Act
            var result = await _sut.GetById(id);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result); 
            var dto = Assert.IsType<ReadServiceRequestDto>(ok.Value);
            Assert.Equal(id, dto.Id);
            Assert.Equal("Läckande tak", dto.Title);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenRequestDoesNotExist()
        {
            // Arrange
            await SeedAsync("Läckande tak");

            // Act
            var result = await _sut.GetById(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetClientHistory_ShouldReturnOnlyThatClientsRequests()
        {
            // Arrange
            await SeedAsync("A", clientId: 1);
            await SeedAsync("B", clientId: 2);

            // Act
            var result = await _sut.GetClientHistory(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<IEnumerable<ReadServiceRequestDto>>(ok.Value);
            var item = Assert.Single(list);
            Assert.Equal("A", item.Title);
        }
    }
}