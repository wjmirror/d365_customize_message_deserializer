using System.Collections.Generic;
namespace D365FOMessageRefactor;

public class PurchaseOrderLevelInformation
{
    public string InvoiceDate { get; set; }
    public string InvoiceId { get; set; }
    public string LedgerVoucher { get; set; }
}

public class LineItem
{
    public string PurchId { get; set; }
    public string ProcurementProductCategoryName { get; set; }
    public string ProcurementProductCategoryHierarchyName { get; set; }
    public int PurchaseLineLineNumber { get; set; }
    public decimal PurchPrice { get; set; }
    public decimal PriceUnit { get; set; }
}

public class PurchaseOrderMessage
{
    public PurchaseOrderLevelInformation PurchaseOrderLevelInformation { get; set; }
    public List<LineItem> LineItems { get; set; }
}
