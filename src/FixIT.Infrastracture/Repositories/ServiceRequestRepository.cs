using FixIT.Domain.Interfaces;
using FixIT.Domain.Models;
using Microsoft.EntityFrameworkCore;
using FixIT.Infrastracture.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace FixIT.Infrastracture.Repositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly FixITDbContext _context;
        public ServiceRequestRepository(FixITDbContext context)
        {
            _context = context;
        }
        public async Task<List<ServiceRequests>> GetAllAsync()
        {
            return await _context.ServiceRequests.ToListAsync();
        }
        public async Task AddAsync(ServiceRequests request)
        {
            _context.ServiceRequests.Add(request);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ServiceRequests>> GetByClientIdAsync(string clientId)
        {
            return await _context.ServiceRequests.Where(r => r.ClientId.ToString() == clientId).ToListAsync();
        }

        public async Task<ServiceRequests?> GetByIdAsync(int id)
        {
            return await _context.ServiceRequests.FindAsync(id);
        }

        public async Task<List<ServiceRequests>> GetByStatusAsync(RequestStatus status)
        {
            return await _context.ServiceRequests.Where(r => r.Status == status).ToListAsync();
        }

        public async Task UpdateAsync(ServiceRequests request)
        {
            _context.ServiceRequests.Update(request);
            await _context.SaveChangesAsync();
        }
    }
}
