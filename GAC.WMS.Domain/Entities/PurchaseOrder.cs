namespace GAC.WMS.Domain.Entities
{
    public class PurchaseOrder : Order
    {
        public List<PurchaseOrderLine>? PurchaseOrderLines { get; set; }
    }
}
