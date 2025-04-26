namespace GAC.WMS.Domain.Entities
{
    public class SaleOrder : Order
    {
        public List<SaleOrderLine>? SaleOrderLines { get; set; }
        public required string ShipmentAddress { get; set; }

    }
}
