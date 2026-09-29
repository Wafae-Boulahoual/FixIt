using FixIT.Application.DTOs;
using FixIT.Application.Interfaces;
using FixIT.Domain.Interfaces;

namespace FixIT.Application.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IServiceRequestRepository _repository;

        public ServiceRequestService(IServiceRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> CreateServiceRequestAsync(CreateServiceRequestDto dto, int clientId)
        {
            if(string.IsNullOrWhiteSpace(dto.Title))
            {
                throw new ArgumentException("Titeln får inte vara tom!");

            }
            if(string.IsNullOrWhiteSpace(dto.Description))
            {
                throw new ArgumentException("Beskrivningen får inte vara tom!");
            }
            if (string.IsNullOrWhiteSpace(dto.Adress))
            {
                throw new ArgumentException("Adressen får inte vara tom!");
            }

            return 1; // temporär
        }


        public Task<List<ReadServiceRequestDto>> GetAllAsync() => throw new NotImplementedException();

        public Task<ReadServiceRequestDto?> GetByIdAsync(int id) => throw new NotImplementedException();

    }
}
