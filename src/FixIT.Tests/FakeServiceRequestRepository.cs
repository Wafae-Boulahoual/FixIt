using FixIT.Domain.Interfaces;
using FixIT.Domain.Models;


namespace FixIT.Tests
{
    // Enkel in-memory-version av repositoryt så vi slipper databas (och mocking-paket) i testerna
    public class FakeServiceRequestRepository : IServiceRequestRepository
    {
        public List<ServiceRequests> Items { get; } = new();
        private int _nextId = 1;

        public Task<List<ServiceRequests>> GetAllAsync() => Task.FromResult(Items.ToList());

        public Task<ServiceRequests?> GetByIdAsync(int id) =>
            Task.FromResult(Items.FirstOrDefault(r => r.Id == id));

        public Task<List<ServiceRequests>> GetByStatusAsync(RequestStatus status) =>
            Task.FromResult(Items.Where(r => r.Status == status).ToList());

        public Task<List<ServiceRequests>> GetByClientIdAsync(string clientId) =>
            Task.FromResult(Items.Where(r => r.ClientId.ToString() == clientId).ToList());

        public Task AddAsync(ServiceRequests request)
        {
            request.Id = _nextId++;
            Items.Add(request);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(ServiceRequests request) => Task.CompletedTask;

        public Task DeleteAsync(int id)
        {
            Items.RemoveAll(r => r.Id == id);
            return Task.CompletedTask;
        }
    }
}