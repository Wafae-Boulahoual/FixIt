using FixIT.Api.Controllers;

namespace FixIT.Tests.IntegrationsTests
{
    public class FixITHealthCheckTests : IClassFixture<CustomWebApplicationFactory<ServiceRequestsController>>
    {
        private readonly HttpClient _client;

        public FixITHealthCheckTests(CustomWebApplicationFactory<ServiceRequestsController> factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Add("X-Api-Key", "test-nyckel"); // samma nyckel som i customwebapplicationfactory
        }

        [Fact] 
        public async Task ApiIsHealthy()
        {
            // Arrange
            var requestUri = "api/servicerequests";

            // act
            var response = await _client.GetAsync(requestUri);

            // Assert

            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            Assert.True(response.Content.Headers.ContentLength > 0);
        }
    }
}