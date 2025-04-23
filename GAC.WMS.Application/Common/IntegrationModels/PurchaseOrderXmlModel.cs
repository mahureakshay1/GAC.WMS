using GAC.WMS.Application.Dtos;
using GAC.WMS.Domain.Entities;
using GAC.WMS.Domain.Enums;
using System.Xml.Serialization;

namespace GAC.WMS.Application.Common.IntegrationModels
{

    [XmlRoot("PurchaseOrderLine")]
    public class PurchaseOrderLineXmlModel
    {
        [XmlElement("ProductId")]
        public int ProductId { get; set; }

        [XmlElement("Quantity")]
        public int Quantity { get; set; }

        [XmlElement("UnitPrice")]
        public decimal UnitPrice { get; set; }

        [XmlElement("TotalPrice")]
        public decimal TotalPrice { get; set; }
    }

    [XmlRoot("PurchaseOrder")]
    public class PurchaseOrderXmlModel
    {
        [XmlElement("ProcessingDate")]
        public DateTime ProcessingDate { get; set; }

        [XmlElement("CustomerId")]
        public int CustomerId { get; set; }

        [XmlElement("Status")]
        public int Status { get; set; }

        [XmlArray("PurchaseOrderLines")]
        [XmlArrayItem("PurchaseOrderLine")]
        public List<PurchaseOrderLineXmlModel>? PurchaseOrderLines { get; set; }
    }
}
