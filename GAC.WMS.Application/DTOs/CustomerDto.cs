namespace GAC.WMS.Application.Dtos
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string? CompanyName { get; set; }
        public string? ContactPersonName { get; set; }
        public string? Address { get; set; }
        public string? Contact { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
