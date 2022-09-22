﻿namespace RoyalERP.API.Contracts.Companies;

public class NewContact {

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string[] Roles { get; set; } = Array.Empty<string>();

}