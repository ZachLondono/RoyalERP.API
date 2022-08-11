﻿namespace RoyalERP.Sales.Companies.DTO;

public class CompanyDTO {

    public Guid Id { get; set; }

    public int Version { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Contact { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public AddressDTO Address { get; set; } = new AddressDTO();

}