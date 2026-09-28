using FixIT.Application.DTOs;
using FixIT.Application.Services;
using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

namespace FixIT.Tests
{
    public class ServiceRequestServiceTests
    {
        
        [Fact]
        public async Task CreateServiceRequestAsync_ShouldThrowExceptionForEmptyTitle()
        {
            // Arrange
            var sut = new ServiceRequestService(); 
            var dto = new CreateServiceRequestDto
            {
                Title = "", // tom titel
                Description = "This is a test request.",
                Adress = "123 Test St"
            };
            int clientId = 1; // t ex

            // Act och Assert
             await Assert.ThrowsAsync<ArgumentException>(() => sut.CreateServiceRequestAsync(dto, clientId));
        }

    }
}
