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
        private readonly FakeServiceRequestRepository _fakeRepository;
        private readonly ServiceRequestsController _sut;

        public ServiceRequestsControllerReadTests()
        {
            _fakeRepository = new FakeServiceRequestRepository();
            var service = new ServiceRequestService(_fakeRepository);
            _sut = new ServiceRequestsController(service);
        }

        private async Task<ServiceRequests> SeedAsync(string title, int clientId = 1)
        {
            var request = new ServiceRequests
            {
                Title = title,
                Description = "Beskrivning för " + title,
                Adress = "Testgatan 1",
                ClientId = clientId
            };
            await _fakeRepository.AddAsync(request); // Id sätts av fake-repot
            return request;
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WithEmptyList_WhenNoRequestsExist() 
        {
            var result = await _sut.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<ReadServiceRequestDto>>(ok.Value);
            Assert.Empty(list);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WithAllRequests() 
        {
            await SeedAsync("Läckande tak");
            await SeedAsync("Trasig dörr");

            var result = await _sut.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<ReadServiceRequestDto>>(ok.Value);
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WithDto_WhenRequestExists() // Testar att hämta en service request som finns
        {
            var saved = await SeedAsync("Läckande tak");

            var result = await _sut.GetById(saved.Id); 

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<ReadServiceRequestDto>(ok.Value);
            Assert.Equal(saved.Id, dto.Id);
            Assert.Equal("Läckande tak", dto.Title);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenRequestDoesNotExist() // Testar att hämta en service request som inte finns
        {
            await SeedAsync("Läckande tak");

            var result = await _sut.GetById(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetClientHistory_ShouldReturnOk_WithOnlyThatClientsRequests()
        {
            await SeedAsync("A", clientId: 1);
            await SeedAsync("B", clientId: 2);

            var result = await _sut.GetClientHistory(1);

            var ok = Assert.IsType<OkObjectResult>(result); 
            var list = Assert.IsType<List<ReadServiceRequestDto>>(ok.Value); 
            var item = Assert.Single(list);
            Assert.Equal("A", item.Title); 
        }
    }
}