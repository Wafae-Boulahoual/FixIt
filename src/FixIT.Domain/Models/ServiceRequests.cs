
namespace FixIT.Domain.Models
{
    public class ServiceRequests
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Adress { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.open;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int ClientId { get; set; }
        public int TechnichianId { get; set; }
    }
}
