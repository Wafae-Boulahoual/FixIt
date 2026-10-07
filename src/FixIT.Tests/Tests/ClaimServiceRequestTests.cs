using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using FixIT.Domain.Models;
using FixIT.Application.Services;
using FixIT.Domain.Interfaces;


namespace FixIT.Tests.Tests
{
    public class ClaimServiceRequestTests
    {
        private readonly Mock<IServiceRequestRepository> _mockRepository;
        private readonly ServiceRequestService _sut;

        public ClaimServiceRequestTests()
        {
            _mockRepository = new Mock<IServiceRequestRepository>();
            _sut = new ServiceRequestService(_mockRepository.Object);
        }

        [Fact]
        public async Task ClaimRequestAsyncShouldSetTechnicianAndStatusInProgress() // teknikerns Id sparas/statusblir pågående/sparas en gång
        {
            // Arrange
            var request = new ServiceRequests
            {
                Id = 1,
                Title = "Läckande tak",
                Description = "Vatten droppar från taket.",
                Adress = "storgatan 1",
                ClientId = "KundNummer-1",
                Status = RequestStatus.open
            };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(request); //Jonas
            // Act
            await _sut.ClaimRequestAsync(1, "teknikerNummer-123");
            // Assert
            Assert.Equal("teknikerNummer-123", request.TechnichianId);
            Assert.Equal(RequestStatus.InProgress, request.Status);
            _mockRepository.Verify(r => r.UpdateAsync(request), Times.Once);
        }

        [Fact]
        public async Task ClaimRequestAsyncShouldThrowWhenRequestNotFound()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ServiceRequests?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.ClaimRequestAsync(99, "teknikerNummer-123"));
        }

        [Fact]
        public async Task ClaimRequestAsyncShouldThrowExceptionIfRequestIsNotOpen()
        {
            //Arrange
            var request = new ServiceRequests
            {
                Id = 2,
                Title = "Trasig dörr",
                Description = "Dörren går inte att öppna.",
                Adress = "storgatan 2",
                ClientId = "KundNummer-2",
                Status = RequestStatus.InProgress,
                TechnichianId = "teknikerNummer-789"
            };
            _mockRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(request);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.ClaimRequestAsync(2, "teknikerNummer-456"));
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<ServiceRequests>()), Times.Never);
        }
    }
}
