using FixIT.Application.DTOs;


namespace FixIT.Application.Interfaces
{
    public interface IServiceRequestService
    {
        Task<int> CreateServiceRequestAsync(CreateServiceRequestDto dto, string clientId); 

        Task<List<ReadServiceRequestDto>> GetAllAsync(); 
        Task<ReadServiceRequestDto?> GetByIdAsync(int id);

        Task<List<ReadServiceRequestDto>> GetClientHistoryAsync(string clientId); 
        Task ClaimRequestAsync(int requestId, string technicianId);
        Task CompleteRequestAsync(int requestId, string technicianId); 
        Task DeleteRequestAsync(int requestId);
        Task UpdateRequestAsync(int requestId, UpdateServiceRequestDto dto, string clientId); //Update, kunden kan ändra ett öppet örende
    }
}
