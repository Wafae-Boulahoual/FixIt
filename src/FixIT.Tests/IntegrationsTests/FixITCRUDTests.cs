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
            _client.DefaultRequestHeaders.Add("X-Api-Key", "test-nyckel"); // samma nyckel som i CustomWebApplicationFactory
        }


        // Helper -
        private async Task<ReadServiceRequestDto?> CreateRequestAsync(string title = "Läckande tak", string clientId = "kund-1")
        {
            var dto = new CreateServiceRequestDto
            {
                Title = title,
                Description = "Vatten droppar från taket.",
                Adress = "Storgatan 1"
            };
            var response = await _client.PostAsJsonAsync($"api/servicerequests?clientId={clientId}", dto);
            return await response.Content.ReadFromJsonAsync<ReadServiceRequestDto>();
        }


        // Create test -
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


        // Read test -
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


        // Update tests -
        [Fact]
        public async Task CanUpdateServiceRequest()
        {
            // Arrange
            var created = await CreateRequestAsync();
            var updatedRequest = new UpdateServiceRequestDto
            {
                Title = "Trasigt tak",
                Description = "Vatten läcker in i köket.",
                Adress = "Storgatan 10"
            };

            // Act
            var updateResponse = await _client.PutAsJsonAsync($"api/servicerequests/{created?.Id}?clientId=kund-1", updatedRequest);
            var getResponse = await _client.GetAsync("api/servicerequests/" + created?.Id);
            var item = await getResponse.Content.ReadFromJsonAsync<ReadServiceRequestDto>();

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
            Assert.Equal("Trasigt tak", item?.Title);
            Assert.Equal("Vatten läcker in i köket.", item?.Description);
            Assert.Equal("Storgatan 10", item?.Adress);
        }

        [Fact]
        public async Task CanNotUpdateServiceRequestWithEmptyTitle()
        {
            // Arrange
            var created = await CreateRequestAsync();
            var updatedRequest = new UpdateServiceRequestDto
            {
                Title = "",
                Description = "Ny beskrivning",
                Adress = "Storgatan 10"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"api/servicerequests/{created?.Id}?clientId=kund-1", updatedRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }


        // Delete tests -
        [Fact]
        public async Task CanDeleteServiceRequest()
        {
            // Arrange
            var created = await CreateRequestAsync();

            // Act
            var deleteResponse = await _client.DeleteAsync("api/servicerequests/" + created?.Id);
            var getResponse = await _client.GetAsync("api/servicerequests/" + created?.Id);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task CanNotDeleteServiceRequestThatDoesNotExist()
        {
            // Act
            var response = await _client.DeleteAsync("api/servicerequests/999999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

       
    }
}