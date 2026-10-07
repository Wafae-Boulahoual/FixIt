using FixIT.Application.DTOs;
using FixIT.Application.Interfaces;
using FixIT.Domain.Interfaces;
using FixIT.Domain.Models;

namespace FixIT.Application.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IServiceRequestRepository _repository;

        public ServiceRequestService(IServiceRequestRepository repository)
        {
            _repository = repository;
        }
        public async Task<int> CreateServiceRequestAsync(CreateServiceRequestDto dto, string clientId)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                throw new ArgumentException("Titeln får inte vara tom!");

            }
            if (string.IsNullOrWhiteSpace(dto.Description))
            {
                throw new ArgumentException("Beskrivningen får inte vara tom!");
            }
            if (string.IsNullOrWhiteSpace(dto.Adress))
            {
                throw new ArgumentException("Adressen får inte vara tom!");
            }

            var request = new ServiceRequests
            {
                Title = dto.Title,
                Description = dto.Description,
                Adress = dto.Adress,
                ClientId = clientId
            };
            await _repository.AddAsync(request);
            return request.Id;
            //return 1; // temporär
        }


        public async Task<List<ReadServiceRequestDto>> GetAllAsync()
        {
            var requests = await _repository.GetAllAsync();
            return requests.Select(MapToDto).ToList();
        }

        private static ReadServiceRequestDto MapToDto(ServiceRequests r) => new()
        {
            Id = r.Id,
            Title = r.Title,
            Description = r.Description,
            Adress = r.Adress,
            Status = r.Status,
            CreatedAt = r.CreatedAt,
            ClientId = r.ClientId,
            TechnichianId = r.TechnichianId
        };
        public async Task<ReadServiceRequestDto?> GetByIdAsync(int id)
        {
            var request = await _repository.GetByIdAsync(id);
            return request is null ? null : MapToDto(request);
        }

        public async Task<List<ReadServiceRequestDto>> GetClientHistoryAsync(string clientId)
        {
            var requests = await _repository.GetByClientIdAsync(clientId);
            return requests.Select(MapToDto).ToList();
        }

        public async Task ClaimRequestAsync(int requestId, string technicianId)
        {
            var request = await _repository.GetByIdAsync(requestId);
            if (request is null)
            {
                throw new KeyNotFoundException("Ärendet finns inte!");
            }
            if(request.Status != RequestStatus.open)
            {
                throw new InvalidOperationException("Ärendet är redan taget!");
            }
            request.TechnichianId = technicianId;
            request.Status = RequestStatus.InProgress;
            await _repository.UpdateAsync(request);
        }
    }
}
