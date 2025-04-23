using GAC.WMS.Domain.Enums;

namespace GAC.WMS.Application.Dtos
{
    public abstract class OrderDto
    {
        public int Id { get; set; }
        public DateTime ProcessingDate { get; set; }
        public int CustomerId { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
