using RoyalERP.Manufacturing.WorkOrders.Domain;

namespace RoyalERP.Manufacturing.WorkOrders.DTO;

public class WorkOrderDTO {

    public Guid Id { get; set; }

    public Guid SalesOrderId { get; set; }

    public string Number { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string VendorName { get; set; } = string.Empty;

    public DateTime? ReleasedDate { get; set; }

    public DateTime? ScheduledDate { get; set; }

    public DateTime? FulfilledDate { get; set; }

    public WorkOrderStatus Status { get; set; }

}