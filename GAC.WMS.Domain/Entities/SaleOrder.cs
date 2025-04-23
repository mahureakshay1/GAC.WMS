namespace GAC.WMS.Domain.Entities
{
    public class SaleOrder : Order
    {
        public IEnumerable<SaleOrderLine>? SaleOrderLines { get; set; }
        public required string ShipmentAddress { get; set; }

    }
}
