using FixIT.Application.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using FixIT.Domain.Interfaces;
using FixIT.Domain.Models;

namespace FixIT.Tests.Tests
{
    public class CompleteServiceRequestTests
    {
        private readonly Mock<IServiceRequestRepository> _mockRepository;
        private readonly ServiceRequestService _sut;
        public CompleteServiceRequestTests()
        {
            _mockRepository = new Mock<IServiceRequestRepository>();
            _sut = new ServiceRequestService(_mockRepository.Object);
        }

        [Fact] // teknikern fixar felet och avslutar ärendet då status blir avslutad och sparas bara en gång
        public async Task CompletedRequestAsyncShouldChangeStatusToCompleted()
        {
            // Arrange
            var request = new ServiceRequests
            {
                Id = 1,
                Title = "Läckande tak",
                Description = "Vatten droppar från taket.",
                Adress = "storgatan 1",
                ClientId = "KundNummer-1",
                Status = RequestStatus.InProgress,
                TechnichianId = "teknikerNummer-123"
            };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(request);
            // Act
            await _sut.CompleteRequestAsync(1, "teknikerNummer-123");
            // Assert
            Assert.Equal(RequestStatus.Completed, request.Status);
            _mockRepository.Verify(r => r.UpdateAsync(request), Times.Once); // sparas en gång
        }


        [Theory] // kan inte avsluta ett ärende som är öppet eller redan avslutat
        [InlineData(RequestStatus.open)]
        [InlineData(RequestStatus.Completed)]
        public async Task CompleteRequestAsyncShouldThrowExceptionIfRequestIsNotInProgress(RequestStatus status)
        {
            // Arrange
            var request = new ServiceRequests
            {
                Id = 2,
                Title = "Trasig dörr",
                Description = "Dörren går inte att öppna.",
                Adress = "storgatan 2",
                ClientId = "KundNummer-2",
                Status = status,
            };
            _mockRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(request);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CompleteRequestAsync(2, "teknikerNummer-456"));
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<ServiceRequests>()), Times.Never); //sparas aldrig
        }
    }
}
