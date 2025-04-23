using GAC.WMS.Domain.Enums;

namespace GAC.WMS.Domain.Entities
{
    public abstract class Order
    {
        public int Id { get; set; }
        public DateTime ProcessingDate { get; set; }
        public OrderStatus Status { get; set; }
        public int CustomerId { get; set; }
        public required Customer Customer { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
