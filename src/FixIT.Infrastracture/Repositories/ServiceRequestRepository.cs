using FixIT.Domain.Interfaces;
using FixIT.Domain.Models;
using FixIT.Infrastracture.Data;
using Microsoft.EntityFrameworkCore;

namespace FixIT.Infrastracture.Repositories
{
    public class ServiceRequestRepository(FixITDbContext context) : IServiceRequestRepository
    {
        public async Task<List<ServiceRequests>> GetAllAsync()
            => await context.ServiceRequests.AsNoTracking().ToListAsync();

        public async Task<ServiceRequests?> GetByIdAsync(int id)
            => await context.ServiceRequests.FindAsync(id);

        public async Task<List<ServiceRequests>> GetByStatusAsync(RequestStatus status)
            => await context.ServiceRequests.AsNoTracking()
                .Where(r => r.Status == status)
                .ToListAsync();

        public async Task<List<ServiceRequests>> GetByClientIdAsync(string clientId)
        {
            // ClientId lagras som int i modellen men interfacet tar emot en string.
            if (!int.TryParse(clientId, out var id))
                return new List<ServiceRequests>();

            return await context.ServiceRequests.AsNoTracking()
                .Where(r => r.ClientId == id)
                .ToListAsync();
        }

        public async Task AddAsync(ServiceRequests request)
        {
            context.ServiceRequests.Add(request);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ServiceRequests request)
        {
            context.ServiceRequests.Update(request);
            await context.SaveChangesAsync();
        }
    }
}