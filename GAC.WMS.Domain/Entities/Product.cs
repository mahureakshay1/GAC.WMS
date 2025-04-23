using GAC.WMS.Domain.Enums;

namespace GAC.WMS.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public DimensionUnit UnitOfDimension { get; set; }
        public int QuantityAvailable { get; set; }
        public QuantityUnit UnitOfQuantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
