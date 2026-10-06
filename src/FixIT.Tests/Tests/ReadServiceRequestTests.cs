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
                ClientId = "1"
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
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllRequests()
        {
            // arrange
            await SeedAsync("Läckande tak");
            await SeedAsync("Trasig dörr");
            await SeedAsync("Sönderfryst rör");

            // act
            var result = await _sut.GetAllAsync();

            // assert
            Assert.Equal(3, result.Count);
            Assert.Contains(result, r => r.Title == "Läckande tak");
            Assert.Contains(result, r => r.Title == "Trasig dörr");
            Assert.Contains(result, r => r.Title == "Sönderfryst rör");
        }

        // ---------- GetByIdAsync ----------
        [Fact]
        public async Task GetByIdAsync_ShouldReturnDto_WhenRequestExists()
        {
            // arrange
            var saved = new ServiceRequests
            {
                Title = "Läckande tak",
                Description = "Vatten droppar i sovrummet",
                Adress = "Storgatan 5",
                Status = RequestStatus.InProgress,
                CreatedAt = new DateTime(2026, 9, 1, 12, 0, 0, DateTimeKind.Utc),
                ClientId = "7",
                TechnichianId = "3"
            };
            await _fakeRepository.AddAsync(saved); // Id blir 1

            //Act
            var result = await _sut.GetByIdAsync(saved.Id);

            // assert – alla fält ska mappas till DTO:n
            Assert.NotNull(result);
            Assert.Equal(saved.Id, result!.Id);
            Assert.Equal("Läckande tak", result.Title);
            Assert.Equal("Vatten droppar i sovrummet", result.Description);
            Assert.Equal("Storgatan 5", result.Adress);
            Assert.Equal(RequestStatus.InProgress, result.Status);
            Assert.Equal(new DateTime(2026, 9, 1, 12, 0, 0, DateTimeKind.Utc), result.CreatedAt);
            Assert.Equal("7", result.ClientId);
            Assert.Equal("3", result.TechnichianId);
        }
         
        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenRequestDoesNotExist()
        {
            // arrange
            await SeedAsync("Läckande tak");

            // act
            var result = await _sut.GetByIdAsync(999);

            // assert
            Assert.Null(result);
        }
        //----------Client history---------------
        [Theory]
        [InlineData("Läckande tak", RequestStatus.open)]
        [InlineData("Vatten droppar i sovrummet", RequestStatus.InProgress)]
        [InlineData("Trasig dörr", RequestStatus.Completed)]
        public async Task GetClientHistory_ShouldReturnRightStatus(string title,RequestStatus status)
        {
            // arrange
            string clientId = "1";
            await _fakeRepository.AddAsync(new ServiceRequests
            {
                Title = title,
                Description = "Testbeskrivning",
                Adress = "Testgatan 1",
                Status = status,
                ClientId = clientId
            });

            // act
            var result = await _sut.GetClientHistoryAsync(clientId);

            // assert
            var request = Assert.Single(result);
            Assert.Equal(title, request.Title);
            Assert.Equal(status, request.Status);
        }
    }
}
