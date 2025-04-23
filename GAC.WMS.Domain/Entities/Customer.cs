namespace GAC.WMS.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public required string  CompanyName { get; set; }
        public required string ContactPersonName { get; set; }
        public required string Address { get; set; }
        public required string Contact { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
