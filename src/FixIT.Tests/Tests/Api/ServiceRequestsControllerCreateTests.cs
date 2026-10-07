using FixIT.Application.DTOs;
using FixIT.Api.Controllers;
using FixIT.Application.Services;
using FixIT.Tests.Fakes;
using Microsoft.AspNetCore.Mvc;

namespace FixIT.Tests.Tests.Api
{
    public class ServiceRequestsControllerCreateTests
    {
        private readonly FakeServiceRequestRepository _repository = new();
        private readonly ServiceRequestsController _sut;

        public ServiceRequestsControllerCreateTests()
        {
            _sut = new ServiceRequestsController(new ServiceRequestService(_repository));
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedWhenDataIsValid()
        {
            //arrange
            var dto = new CreateServiceRequestDto
            {
                Title = "Läckande tak",
                Description = " vatten droppar från taket.",
                Adress = "storgatan 20"
            };

            //act
            var actual = await _sut.Create(dto, "kundNummer-1");

            //assert
            Assert.IsType<CreatedResult>(actual);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequestWhenTitleIsEmpty()
        {
            //arrange
            var dto = new CreateServiceRequestDto
            {
                Title = "",
                Description = " vatten droppar från taket.",
                Adress = "storgatan 20"
            };

            //act
            var actual = await _sut.Create(dto, "kundNummer-1");

            //assert
            Assert.IsType<BadRequestObjectResult>(actual);
        }
    }
}
