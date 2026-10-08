using FixIT.Application.DTOs;
using FixIT.Application.Services;
using FixIT.Domain.Interfaces;
using FixIT.Domain.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace FixIT.Tests.Tests
{
    public class UpdateServiceRequestTests
    {
        private readonly Mock<IServiceRequestRepository> _mockRepository;
        private readonly ServiceRequestService _sut;
        public UpdateServiceRequestTests()
        {
            _mockRepository = new Mock<IServiceRequestRepository>();
            _sut = new ServiceRequestService(_mockRepository.Object);
        }

        [Fact]
        public async Task UpdateRequestAsyncShouldUpdateRequestIfOpen()
        {
            // Arrange
            var request = new ServiceRequests
            {
                Id = 1,
                Title = "Läckande tak",
                Description = "Vatten droppar.",
                Adress = "storgatan 1",
                ClientId = "KundNummer-1",
                Status = RequestStatus.open
            };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(request);

            var dto = new UpdateServiceRequestDto
            {
                Title = "Läckande tak i köket",
                Description = "Vatten droppar mycket.",
                Adress = "storgatan 10"
            };

            // Act

            await _sut.UpdateRequestAsync(1, dto, "KundNummer-1");

            // Assert

            Assert.Equal("Läckande tak i köket", request.Title);
            Assert.Equal("Vatten droppar mycket.", request.Description);
            Assert.Equal("storgatan 10", request.Adress);
            _mockRepository.Verify(r => r.UpdateAsync(request), Times.Once); // sparas en gång
        }

        [Theory] // pågående eller avslutade ärende kan inte ändras
        [InlineData(RequestStatus.InProgress)]
        [InlineData(RequestStatus.Completed)]
        public async Task UpdateRequestAsyncShouldThrowExceptionIfNotOpen(RequestStatus status)
        {
            // Arrange
            var request = new ServiceRequests
            {
                Id = 2,
                Title = "Trasig dörr",
                Description = "Dörren går inte att öppna.",
                Adress = "storgatan 2",
                ClientId = "KundNummer-1",
                Status = status
            };
            _mockRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(request);

            var dto = new UpdateServiceRequestDto
            {
                Title = "Ny titel",
                Description = "Ny beskrivning",
                Adress = "Ny adress"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.UpdateRequestAsync(2, dto, "KundNummer-1"));
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<ServiceRequests>()), Times.Never); // sparas aldrig
        }

        [Fact] // en annan kund kan inte ändra ärendet
        public async Task UpdateRequestAsyncShouldThrowExceptionIfWrongClient()
        {
            // Arrange
            var request = new ServiceRequests
            {
                Id = 3,
                Title = "Trasigt fönster",
                Description = "Fönstret går inte att stänga.",
                Adress = "storgatan 3",
                ClientId = "KundNummer-1",
                Status = RequestStatus.open
            };
            _mockRepository.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(request);

            var dto = new UpdateServiceRequestDto
            {
                Title = "Ny titel",
                Description = "Ny beskrivning",
                Adress = "Ny adress"
            };

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.UpdateRequestAsync(3, dto, "KundNummer-2"));
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<ServiceRequests>()), Times.Never); // sparas aldrig
        }


        [Theory] // titeln får inte vara tom
        [InlineData("")]
        [InlineData("   ")]
        public async Task UpdateRequestAsyncShouldThrowExceptionIfTitleIsEmpty(string title)
        {
            // Arrange
            var request = new ServiceRequests
            {
                Id = 4,
                Title = "Läckande kran",
                Description = "Kranen droppar.",
                Adress = "storgatan 4",
                ClientId = "KundNummer-1",
                Status = RequestStatus.open
            };
            _mockRepository.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(request);

            var dto = new UpdateServiceRequestDto
            {
                Title = title,
                Description = "Ny beskrivning",
                Adress = "Ny adress"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.UpdateRequestAsync(4, dto, "KundNummer-1"));
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<ServiceRequests>()), Times.Never); 
        }
    }
  
}
