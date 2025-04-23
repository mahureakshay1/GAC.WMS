namespace GAC.WMS.Application.Dtos
{
    public class PurchaseOrderDto : OrderDto
    {
        public required IEnumerable<PurchaseOrderLineDto> PurchaseOrderLines { get; set; }
    }
}
