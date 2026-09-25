using FixIT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FixIT.Domain.Interfaces
{
    public interface IServiceRequestRepository
    {
        Task<List<ServiceRequests>> GetAllAsync(); // alla ärenden => Read
        Task<ServiceRequests?> GetByIdAsync(int id); // detaglier om ett specifikt ärendet => Read
        Task<List<ServiceRequests>> GetByStatusAsync(RequestStatus status); // alla ärenden med en viss status (typ bara de öppna) => Read
        Task<List<ServiceRequests>> GetByClientIdAsync(string clientId); // alla ärenden för en viss kund => Read
        Task AddAsync(ServiceRequests request); // skapa/spara ett nytt ärende => Create
        Task UpdateAsync(ServiceRequests request); // uppdatera ett befintligt ärende => Update

        // Task DeleteAsync(int id); // ta bort ett ärende (om vi ska behöva den senare) => Delete
    }
}
