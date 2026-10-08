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
          
            ValidateFields(dto.Title, dto.Description, dto.Adress);
            var request = new ServiceRequests
            {
                Title = dto.Title,
                Description = dto.Description,
                Adress = dto.Adress,
                ClientId = clientId
            };
            await _repository.AddAsync(request);
            return request.Id;
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
            var request = await GetRequestOrThrowAsync(requestId);
            if (request.Status != RequestStatus.open)
            {
                throw new InvalidOperationException("Ärendet är redan taget!");
            }
            request.TechnichianId = technicianId;
            request.Status = RequestStatus.InProgress;
            await _repository.UpdateAsync(request);
        }
        public async Task CompleteRequestAsync(int requestId, string technicianId)
        {
            var request = await GetRequestOrThrowAsync(requestId);
            if (request.Status != RequestStatus.InProgress)
            {
                throw new InvalidOperationException("Ärendet är inte pågående!");
            }
            if (request.TechnichianId != technicianId)
            {
                throw new UnauthorizedAccessException("Du kan bara avsluta dina ärenden!");
            }
            request.Status = RequestStatus.Completed;
            await _repository.UpdateAsync(request);
        }

        public async Task DeleteRequestAsync(int requestId)
        {
            await GetRequestOrThrowAsync(requestId);
            await _repository.DeleteAsync(requestId);
        }
        public async Task UpdateRequestAsync(int requestId, UpdateServiceRequestDto dto, string clientId)
        {
            var request = await GetRequestOrThrowAsync(requestId);
            if (request.Status != RequestStatus.open)
            {
                throw new InvalidOperationException("Ärendet kan inte ändras!");
            }
            if (request.ClientId != clientId)
            {
                throw new UnauthorizedAccessException("Du kan bara ändra dina egna ärenden!");
            }
            
            ValidateFields(dto.Title, dto.Description, dto.Adress);
            request.Title = dto.Title;
            request.Description = dto.Description;
            request.Adress = dto.Adress;

            await _repository.UpdateAsync(request);
        }



        private static void ValidateFields(string title, string description, string adress) // kollar att fälten inte är tomma
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Titeln får inte vara tom!");
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Beskrivningen får inte vara tom!");
            }
            if (string.IsNullOrWhiteSpace(adress))
            {
                throw new ArgumentException("Adressen får inte vara tom!");
            }
        }

        private async Task<ServiceRequests> GetRequestOrThrowAsync(int requestId) // hämtar ärendet eller kastar fel
        {
            var request = await _repository.GetByIdAsync(requestId);
            if (request is null)
            {
                throw new KeyNotFoundException("Ärendet finns inte!");
            }
            return request;
        }

    }
}
