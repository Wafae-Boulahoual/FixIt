using FixIT.Application.Services;
using FixIT.Domain.Models;

namespace FixIT.Tests
{
    public class ServiceRequestServiceReadTests
    {
        private readonly FakeServiceRequestRepository _repo = new();
        private readonly ServiceRequestService _sut;

        public ServiceRequestServiceReadTests()
        {
            _sut = new ServiceRequestService(_repo);
        }

        private void Seed(int id, string title = "Läckande tak")
        {
            _repo.Items.Add(new ServiceRequests
            {
                Id = id,
                Title = title,
                Description = "Vattentäcka som måste fixas",
                Adress = "Testgatan 123",
                Status = RequestStatus.open,
                ClientId = 1
            });
        }
        //--------Get All--------
        [Fact]
        public async Task GetAll_ShouldReturnEmptyList_WhenNoRequestsExist()
        {
            //Arange
            var result = await _sut.GetAllAsync();

            //Assert
            Assert.Empty(result);
        }


    }
}
