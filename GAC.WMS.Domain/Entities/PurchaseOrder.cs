namespace GAC.WMS.Domain.Entities
{
    public class PurchaseOrder : Order
    {
        public IEnumerable<PurchaseOrderLine>? PurchaseOrderLines { get; set; }
    }
}
