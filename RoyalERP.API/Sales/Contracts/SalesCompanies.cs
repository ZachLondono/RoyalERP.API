using RoyalERP.API.Contracts.Companies;

namespace RoyalERP.API.Sales.Contracts;

public static class SalesCompanies {

    public delegate Task<CompanyDTO?> GetCompanyById(Guid Id);

}
