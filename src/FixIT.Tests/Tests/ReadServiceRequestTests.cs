using FixIT.Tests.Fakes;
using FixIT.Application.Services;
using FixIT.Domain.Models;

namespace FixIT.Tests.Tests
{
    public class ReadServiceRequestTests
    {
        private readonly FakeServiceRequestRepository _fakeRepository;
        private readonly ServiceRequestService _sut;

        public ReadServiceRequestTests()
        {
            _fakeRepository = new FakeServiceRequestRepository();
            _sut = new ServiceRequestService(_fakeRepository);
        }

        private async Task<ServiceRequests> SeedAsync(string title, string adress = "Testgatan 1")
        {
            var request = new ServiceRequests
            {
                Title = title,
                Description = "Beskrivning för " + title,
                Adress = adress,
                ClientId = 1
            };
            await _fakeRepository.AddAsync(request);
            return request;
        }


        // ---------- GetAllAsync ----------
        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoRequestsExist()
        {
            //Act
            var result = await _sut.GetAllAsync();

            //Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
