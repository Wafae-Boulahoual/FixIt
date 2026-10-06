using FixIT.Application.DTOs;
using FixIT.Application.Services;
using FixIT.Tests.Fakes;
using System;
using System.Collections.Generic;
using System.Text;

namespace FixIT.Tests.Tests
{
    public class CreateServiceRequestTests
    {
        private readonly FakeServiceRequestRepository _fakeRepository;
        private readonly ServiceRequestService _sut;
        public CreateServiceRequestTests()
        {
            _fakeRepository = new FakeServiceRequestRepository();
            _sut = new ServiceRequestService(_fakeRepository);
        }
        [Theory]
        [InlineData("")]
        [InlineData("     ")]
        [InlineData(null)]
        public async Task CreateServiceRequestAsync_ShouldThrowExceptionIfTitleIsEmpty(string title)
        {
            //arrange

            var dto = new CreateServiceRequestDto
            {
                Title = title,
                Description = "We test the service request",
                Adress = " test address 123 "

            };
            string clientId = "1";

            //act och assert
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateServiceRequestAsync(dto, clientId));
        }
        [Theory]
        [InlineData("")]
        [InlineData("     ")]
        [InlineData(null)]
        public async Task CreateServiceRequestAsync_ShouldThrowExceptionIfDescriptionIsEmpty(string description)
        {
            //arrange
            var dto = new CreateServiceRequestDto
            {
                Title = "en titel",
                Description = description,
                Adress = " test address 123 "
            };
            string clientId = "1";

            //act och assert
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateServiceRequestAsync(dto, clientId));
        }

        [Theory]
        [InlineData("")]
        [InlineData("     ")]
        [InlineData(null)]
        public async Task CreateServiceRequestAsync_ShouldThrowExceptionIfAdressIsEmpty(string adress)
        {
            //arrange
            var dto = new CreateServiceRequestDto
            {
                Title = "en titel",
                Description = "vattentäcka som måste fixas",
                Adress = adress
            };
            string clientId = "1";

            //act och assert
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateServiceRequestAsync(dto, clientId));
        }
        [Fact]
        public async Task CreateServiceRequestAsync_ShouldSaveAndReturnIdIfDataIsCorrect()
        {
            //arrange
            var dto = new CreateServiceRequestDto
            {
                Title = "en titel",
                Description = "vattentäcka som måste fixas",
                Adress = "test address 123"
            };
            string clientId = "1";
            //act
            var id = await _sut.CreateServiceRequestAsync(dto, clientId);
            //assert
            Assert.Equal(1, id);
            var savedRequest = Assert.Single(_fakeRepository.Saved);
            Assert.Equal("en titel", savedRequest.Title);
            Assert.Equal("vattentäcka som måste fixas", savedRequest.Description);
            Assert.Equal("test address 123", savedRequest.Adress);
            Assert.Equal(clientId, savedRequest.ClientId);
        }


    }
}
