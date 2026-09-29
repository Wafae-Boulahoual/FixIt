using FixIT.Application.DTOs;
using FixIT.Application.Services;


namespace FixIT.Tests
{
    public class ServiceRequestServiceTests
    {
        [Fact] // felanmälan med tom titel*
        public async Task CreateServiceRequestAsync_ShouldThrowExceptionIfTitleIsEmpty()
        {
            //arrange
            var sut = new ServiceRequestService(new FakeServiceRequestRepository());
            
            var dto = new CreateServiceRequestDto
            {
                Title= "",
                Description = "We test the service request",
                Adress = " test address 123 "

            };
            int clientId = 1;
            
            //act och assert
            await Assert.ThrowsAsync<ArgumentException>(() => sut.CreateServiceRequestAsync(dto, clientId));
        }
        [Theory]
        [InlineData("")]
        [InlineData("     ")]
        public async Task CreateServiceRequestAsync_ShouldThrowExceptionIfDescriptionIsEmpty(string description)
        { 
            //arrange
            var sut = new ServiceRequestService(new FakeServiceRequestRepository());
            var dto = new CreateServiceRequestDto
            {
                Title = "en titel",
                Description = description,
                Adress = " test address 123 "
            };
            int clientId = 1;

            //act coh assert
            await Assert.ThrowsAsync<ArgumentException>(() => sut.CreateServiceRequestAsync(dto, clientId));
        }

        [Theory]
        [InlineData("")]
        [InlineData("     ")]
        public async Task CreateServiceRequestAsync_ShouldThrowExceptionIfAdressIsEmpty(string adress)
        {
            //arrange
            var sut = new ServiceRequestService(new FakeServiceRequestRepository());
            var dto = new CreateServiceRequestDto
            {
                Title = "en titel",
                Description = "vattentäcka som måste fixas",
                Adress = adress
            };
            int clientId = 1;

            //act coh assert
            await Assert.ThrowsAsync<ArgumentException>(() => sut.CreateServiceRequestAsync(dto, clientId));
        }


    }
}
