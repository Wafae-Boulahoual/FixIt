using FixIT.Application.Services;
using Moq;
using FixIT.Domain.Interfaces;
using FixIT.Domain.Models;

namespace FixIT.Tests.Tests
{
    public class DeleteServiceRequestTests
    {
        private readonly Mock<IServiceRequestRepository> _mockRepository;
        private readonly ServiceRequestService _sut;
        public DeleteServiceRequestTests()
        {
            _mockRepository = new Mock<IServiceRequestRepository>();
            _sut = new ServiceRequestService(_mockRepository.Object);  //object är en instans av mocken som implementerar interfacet
        }
        

        [Theory] // admin kan ta bort ett ärende oavsett status
        [InlineData(RequestStatus.open)]
        [InlineData(RequestStatus.InProgress)]
        [InlineData(RequestStatus.Completed)]
        public async Task DeleteRequestAsyncShouldCallRepositoryDelete(RequestStatus status)
        {
            // Arrange
            var request = new ServiceRequests
            {
                Id = 1,
                Title = "Läckande tak",
                Description = "Vatten droppar från taket.",
                Adress = "storgatan 1",
                ClientId = "KundNummer-1",
                Status = status
            };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(request);

            // Act
            await _sut.DeleteRequestAsync(1);

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once); // tas bort en gång
        }

        [Fact] // ärendet finns inte
        public async Task DeleteRequestAsyncShouldThrowExceptionIfRequestDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ServiceRequests?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteRequestAsync(99));
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never); // tas aldrig bort
        }
    }
}
