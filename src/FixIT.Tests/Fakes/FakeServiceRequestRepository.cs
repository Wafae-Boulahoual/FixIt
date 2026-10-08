using FixIT.Domain.Interfaces;
using FixIT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FixIT.Tests.Fakes
{
    public class FakeServiceRequestRepository : IServiceRequestRepository
    {
        public List<ServiceRequests> Saved = new();
        public Task AddAsync(ServiceRequests request)
        {
            request.Id = Saved.Count + 1;
            Saved.Add(request);
            return Task.CompletedTask;
        }

        public Task<List<ServiceRequests>> GetAllAsync()
        {
            return Task.FromResult(Saved);
        }

        public Task<List<ServiceRequests>> GetByClientIdAsync(string clientId)
        {
            return Task.FromResult(Saved.Where(r => r.ClientId == clientId).ToList());
        }

        public Task<ServiceRequests?> GetByIdAsync(int id)
        {
            return Task.FromResult(Saved.FirstOrDefault(r => r.Id == id));
        }

        public Task<List<ServiceRequests>> GetByStatusAsync(RequestStatus status)
        {
            return Task.FromResult(Saved.Where(r => r.Status == status).ToList());
        }

        public Task UpdateAsync(ServiceRequests request)
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            Saved.RemoveAll(r => r.Id == id);
            return Task.CompletedTask;
        }
    }

}
