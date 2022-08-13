﻿namespace RoyalERP.Manufacturing.WorkOrders.DTO;

public class NewWorkOrder {

    public Guid SalesOrderId { get; set; }

    public string Number { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public string VendorName { get; set; } = string.Empty;

}