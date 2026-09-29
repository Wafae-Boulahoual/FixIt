using FixIT.Application.DTOs;


namespace FixIT.Application.Interfaces
{
    public interface IServiceRequestService
    {
        Task<int> CreateServiceRequestAsync(CreateServiceRequestDto dto, int clientId); //Skapa en ny felanmälan och returnera sin ID 

        Task<List<ReadServiceRequestDto>> GetAllAsync();   // Read – alla ärenden
        Task<ReadServiceRequestDto?> GetByIdAsync(int id); // Read – ett ärende, null om det saknas
    }
}
