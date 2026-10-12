using FixIT.Api.Controllers;
using FixIT.Application.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace FixIT.Tests.IntegrationsTests
{
    public class FixITCRUDTests : IClassFixture<CustomWebApplicationFactory<ServiceRequestsController>>
    {
        private readonly HttpClient _client;

        public FixITCRUDTests(CustomWebApplicationFactory<ServiceRequestsController> factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Add("X-Api-Key", "test-nyckel"); // samma nyckel som i fabriken
        }

        [Fact]
        public async Task CanCreateServiceRequest()
        {
            // Arrange
            var newRequest = new CreateServiceRequestDto
            {
                Title = "Läckande tak",
                Description = "Vatten droppar från taket.",
                Adress = "Storgatan 1"
            };

            // Act
            var response = await _client.PostAsJsonAsync("api/servicerequests?clientId=kund-1", newRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<ReadServiceRequestDto>();
            Assert.True(created?.Id > 0);
        }

        [Fact]
        public async Task CanNotCreateServiceRequestWithoutTitle()
        {
            // Arrange
            var newRequest = new CreateServiceRequestDto
            {
                Title = "",
                Description = "Vatten droppar från taket.",
                Adress = "Storgatan 1"
            };

            // Act
            var response = await _client.PostAsJsonAsync("api/servicerequests?clientId=kund-1", newRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CanReadServiceRequest()
        {
            // Arrange
            var newRequest = new CreateServiceRequestDto
            {
                Title = "Trasig dörr",
                Description = "Dörren går inte att stänga.",
                Adress = "Storgatan 2"
            };
            var createResponse = await _client.PostAsJsonAsync("api/servicerequests?clientId=kund-1", newRequest);
            var created = await createResponse.Content.ReadFromJsonAsync<ReadServiceRequestDto>();

            // Act
            var getResponse = await _client.GetAsync("api/servicerequests/" + created?.Id);
            var item = await getResponse.Content.ReadFromJsonAsync<ReadServiceRequestDto>();

            // Assert
            getResponse.EnsureSuccessStatusCode();
            Assert.Equal("Trasig dörr", item?.Title);
            Assert.Equal("Dörren går inte att stänga.", item?.Description);
            Assert.Equal("Storgatan 2", item?.Adress);
            Assert.Equal("kund-1", item?.ClientId);
        }
    }
}