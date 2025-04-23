namespace GAC.WMS.Application.Dtos
{
    public class SellOrderDto : OrderDto
    {
        public required IEnumerable<SaleOrderLineDto> SaleOrderLines { get; set; }
        public required string ShipmentAddress { get; set; }

    }
}
