using FixIT.Domain.Models;
namespace FixIT.Application.DTOs
{
    public class ReadServiceRequestDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Adress { get; set; } = "";
        public RequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public string ClientId { get; set; } = "";
        public string? TechnichianId { get; set; }
    }
}
