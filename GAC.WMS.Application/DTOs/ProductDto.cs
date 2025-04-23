using GAC.WMS.Domain.Enums;

namespace GAC.WMS.Application.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
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
